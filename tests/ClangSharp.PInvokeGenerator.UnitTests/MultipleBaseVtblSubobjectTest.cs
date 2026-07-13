// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using ClangSharp.UnitTests.Baseline;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/592.
/// A struct deriving from two or more bases that each carry a virtual table has a distinct vtable
/// pointer per base subobject in the native ABI. Only the primary (first) polymorphic base shares its
/// vtable with the derived type and is flattened into the single <c>lpVtbl</c>; every other polymorphic
/// base is emitted as a nested subobject field so its own vtable pointer -- and therefore correct
/// dispatch -- is preserved. Previously all bases were flattened onto a single <c>lpVtbl</c>, which both
/// duplicated members that mapped to the same name (a CS0111 compile error) and silently dispatched a
/// secondary base's methods through the primary vtable.
/// </summary>
[Platform("win")]
public sealed class MultipleBaseVtblSubobjectTest : StandaloneBaselineTest
{
    protected override string Area => "MultipleBaseVtblSubobject";

    private const string InputContents = @"struct Foo
{
    virtual ~Foo() = default;
};

struct Bar
{
    virtual ~Bar() = default;
};

struct Baz : Foo, Bar
{
};
";

    [Test]
    public Task EmitsSecondaryBaseAsSubobject()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents);
    }

    [Test]
    public Task EmitsSecondaryBaseAsSubobjectWithExplicitVtbls()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(InputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);
    }

    // The non-unifying case: two bases contribute distinctly named virtual methods and the derived type
    // introduces one of its own. Only the primary base (`Foo`) is flattened, so `Foo::FooMethod` and the
    // derived `Baz::BazMethod` share `Baz`'s `lpVtbl` (indices 0 and 1). `Bar::BarMethod` lives in `Bar`'s
    // own vtable and is dispatched through the `Base2` subobject, not the primary `lpVtbl[0]`.
    private const string NonUnifyingInputContents = @"struct Foo
{
    virtual void FooMethod();
};

struct Bar
{
    virtual void BarMethod();
};

struct Baz : Foo, Bar
{
    virtual void BazMethod();
};
";

    [Test]
    public Task DispatchesNonPrimaryBaseThroughSubobject()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(NonUnifyingInputContents);
    }

    [Test]
    public Task InheritsPrimaryBaseMarkerInterfaceOnly()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(NonUnifyingInputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls | PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);
    }

    // A field-bearing polymorphic primary base cannot be flattened into the derived type's `lpVtbl` (the
    // derived type would have to inline its fields), so both it and the secondary polymorphic base are
    // emitted as subobjects and the derived type declares no `lpVtbl` of its own.
    private const string FieldBearingPrimaryInputContents = @"struct Foo
{
    int x;

    virtual void A();
};

struct Bar
{
    virtual void B();
};

struct Baz : Foo, Bar
{
};
";

    [Test]
    public Task EmitsFieldBearingPrimaryBaseAsSubobject()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(FieldBearingPrimaryInputContents);
    }

    // Nested multiple inheritance: `C` derives from two polymorphic bases and `D` derives from `C`.
    // `C` is a single level of multiple inheritance and is modeled exactly (`A` flattened, `B` a subobject).
    // Flattening `C` into `D`, however, would require carrying `C`'s own non-primary polymorphic base (`B`,
    // at its own vtable pointer offset) onto `D`, which is not yet modeled. `D` therefore falls back to the
    // legacy run-on `lpVtbl` -- every inherited method stays reachable (so the bindings still compile), but
    // a secondary base's methods dispatch through the primary vtable, so the record is flagged incomplete.
    private const string NestedMultipleInheritanceInputContents = @"struct A
{
    virtual void a();
};

struct B
{
    virtual void b();
};

struct C : A, B
{
    virtual void c();
};

struct D : C
{
    virtual void d();
};
";

    [Test]
    public Task FallsBackToFlattenedVtblForNestedMultipleInheritance()
    {
        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Unsupported cxx record declaration: 'nested multiple virtual bases'. Generated bindings for D may be incomplete.", "Line 16, Column 8 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(NestedMultipleInheritanceInputContents, expectedDiagnostics: expectedDiagnostics);
    }
}
