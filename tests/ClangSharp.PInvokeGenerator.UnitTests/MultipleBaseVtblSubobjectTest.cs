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
    // `D` flattens `C`'s primary chain (`A`, plus `C`'s and `D`'s own methods) into its single `lpVtbl` and
    // carries `C`'s non-primary polymorphic base `B` onto itself as a subobject, exactly mirroring the
    // single-level layout. `b()` is reached through the `B` subobject and is not re-exposed on `D`.
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
    public Task CarriesNestedNonPrimaryBaseAsSubobject()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(NestedMultipleInheritanceInputContents);
    }

    // Double carry: `C` derives from three polymorphic bases, so it flattens `A` and carries both `B` and
    // `E` as subobjects. `D : C` must carry both onto itself with the same, non-colliding names (`Base2`,
    // `Base3`) it uses on `C` -- numbering the carried bases by their owning record (`C`) rather than by
    // `D`'s direct bases is what keeps them distinct.
    private const string DoubleCarryInputContents = @"struct A
{
    virtual void AMethod();
};

struct B
{
    virtual void BMethod();
};

struct E
{
    virtual void EMethod();
};

struct C : A, B, E
{
};

struct D : C
{
};
";

    [Test]
    public Task CarriesMultipleNestedNonPrimaryBasesWithStableNames()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(DoubleCarryInputContents);
    }

    // The carried subobject propagates through an arbitrary depth of single-inheritance chaining: `F : D`
    // flattens the whole primary chain (`a`, `c`, `d`, `f`) into its `lpVtbl` and still carries `B`.
    private const string DeeplyNestedInputContents = @"struct A
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

struct F : D
{
    virtual void f();
};
";

    [Test]
    public Task CarriesNestedNonPrimaryBaseThroughDeepChain()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(DeeplyNestedInputContents);
    }

    // A field-bearing polymorphic primary base shares (and physically holds) the derived type's vtable
    // pointer, so the derived type must NOT declare a second `lpVtbl` -- that would corrupt its layout and
    // dispatch through an uninitialized pointer. Instead the base is emitted as a subobject and every
    // virtual method dispatches through the pointer that lives inside it (`Base.lpVtbl`), keeping native
    // vtable indices (`A` at `0`, `c` at `1`).
    private const string FieldBearingPrimaryWithMethodsInputContents = @"struct Foo
{
    int x;

    virtual void A();
};

struct C : Foo
{
    virtual void c();
};
";

    [Test]
    public Task SharesFieldBearingPrimaryBaseVtblPointer()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(FieldBearingPrimaryWithMethodsInputContents);
    }

    [Test]
    public Task SharesFieldBearingPrimaryBaseVtblPointerWithExplicitVtbls()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(FieldBearingPrimaryWithMethodsInputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);
    }

    // The shared pointer is reached through as many field-bearing primary subobjects as the chain is deep:
    // `D : C : Foo` dispatches through `Base.Base.lpVtbl`, still at native indices.
    private const string FieldBearingPrimaryDeepChainInputContents = @"struct Foo
{
    int x;

    virtual void A();
};

struct C : Foo
{
    virtual void c();
};

struct D : C
{
    virtual void d();
};
";

    [Test]
    public Task SharesFieldBearingPrimaryBaseVtblPointerThroughDeepChain()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(FieldBearingPrimaryDeepChainInputContents);
    }

    // MSVC promotes the first polymorphic base to offset 0 so the object can share its vtable pointer,
    // reordering any non-polymorphic base that was declared before it. The subobject fields must be emitted
    // in native layout order (`P` first, then `Data`) rather than declaration order, or the struct layout
    // would not match the native ABI.
    private const string ReorderedPolymorphicBaseInputContents = @"struct Data
{
    int dx;
};

struct P
{
    int pp;
    virtual void v();
};

struct C : Data, P
{
    int cx;
    virtual void c();
};
";

    [Test]
    public Task EmitsReorderedPolymorphicPrimaryBaseInLayoutOrder()
    {
        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(ReorderedPolymorphicBaseInputContents);
    }

    // Virtual inheritance is not representable in the by-value subobject model: a virtually-inherited base
    // is laid out once for the most-derived object, so its offset within a given subobject is
    // context-dependent. The generator emits a diagnostic and falls back to a best-effort layout rather
    // than silently emitting an ABI-incorrect one.
    private const string VirtualInheritanceInputContents = @"struct A
{
    int ax;
    virtual void a();
};

struct C : virtual A
{
    int cx;
    virtual void c();
};
";

    [Test]
    public Task WarnsThatVirtualInheritanceIsNotModeled()
    {
        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Virtual inheritance is not currently modeled. The generated layout and vtable dispatch for this type may be incorrect.", "Line 7, Column 8 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBaselineAsync(VirtualInheritanceInputContents, expectedDiagnostics: expectedDiagnostics);
    }
}
