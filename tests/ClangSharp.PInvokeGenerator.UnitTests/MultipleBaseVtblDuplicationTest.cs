// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
using NUnit.Framework;

namespace ClangSharp.UnitTests;

/// <summary>
/// Regression test for https://github.com/dotnet/ClangSharp/issues/592.
/// A struct deriving from two or more bases that each carry a virtual table flattens every base into
/// the single <c>lpVtbl</c>. Members from different bases that map to the same C# name and signature
/// (most notably each base's virtual destructor becoming <c>Dispose</c>) were emitted more than once,
/// producing a CS0111 compile error. The duplicated member is now emitted only once. The generated
/// bindings remain incomplete for multiple virtual bases (a warning is still reported), because
/// correctly modeling that requires a distinct vtable pointer per base subobject.
/// </summary>
[Platform("win")]
public sealed class MultipleBaseVtblDuplicationTest : PInvokeGeneratorTest
{
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
    public Task DoesNotEmitDuplicateMembers()
    {
        var expectedOutputContents = @"using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo
    {
        public void** lpVtbl;

        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<Foo*, void>)(lpVtbl[0]))((Foo*)Unsafe.AsPointer(ref this));
        }
    }

    public unsafe partial struct Bar
    {
        public void** lpVtbl;

        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<Bar*, void>)(lpVtbl[0]))((Bar*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName(""struct Baz : Foo, Bar"")]
    public unsafe partial struct Baz
    {
        public void** lpVtbl;

        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<Baz*, void>)(lpVtbl[0]))((Baz*)Unsafe.AsPointer(ref this));
        }
    }
}
";

        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Unsupported cxx record declaration: 'multiple virtual bases'. Generated bindings may be incomplete.", "Line 11, Column 8 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(InputContents, expectedOutputContents, expectedDiagnostics: expectedDiagnostics);
    }

    [Test]
    public Task DoesNotEmitDuplicateExplicitVtblEntries()
    {
        var expectedOutputContents = @"using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo
    {
        public Vtbl* lpVtbl;

        public void Dispose()
        {
            lpVtbl->Dispose((Foo*)Unsafe.AsPointer(ref this));
        }

        public partial struct Vtbl
        {
            [NativeTypeName(""void () noexcept"")]
            public delegate* unmanaged[Thiscall]<Foo*, void> Dispose;
        }
    }

    public unsafe partial struct Bar
    {
        public Vtbl* lpVtbl;

        public void Dispose()
        {
            lpVtbl->Dispose((Bar*)Unsafe.AsPointer(ref this));
        }

        public partial struct Vtbl
        {
            [NativeTypeName(""void () noexcept"")]
            public delegate* unmanaged[Thiscall]<Bar*, void> Dispose;
        }
    }

    [NativeTypeName(""struct Baz : Foo, Bar"")]
    public unsafe partial struct Baz
    {
        public Vtbl* lpVtbl;

        public void Dispose()
        {
            lpVtbl->Dispose((Baz*)Unsafe.AsPointer(ref this));
        }

        public partial struct Vtbl
        {
            [NativeTypeName(""void () noexcept"")]
            public delegate* unmanaged[Thiscall]<Baz*, void> Dispose;
        }
    }
}
";

        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Unsupported cxx record declaration: 'multiple virtual bases'. Generated bindings may be incomplete.", "Line 11, Column 8 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(InputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls, expectedDiagnostics: expectedDiagnostics);
    }

    // The non-unifying case: two bases contributing distinctly named virtual methods, plus a virtual
    // method introduced by the derived type itself. There is no name collision here, so nothing is
    // deduplicated, but the flattened single-vtable model is still wrong: the secondary base's
    // `BarMethod` is dispatched through `lpVtbl[0]` (the primary vtable slot shared with `FooMethod`)
    // rather than the Bar subobject's own vtable. Because the derived type introduces its own virtual
    // (`BazMethod`), this case previously produced NO diagnostic at all; the warning must still fire so
    // the incompleteness is never silent.
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
    public Task WarnsForNonUnifyingMultipleBases()
    {
        var expectedOutputContents = @"using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo
    {
        public void** lpVtbl;

        public void FooMethod()
        {
            ((delegate* unmanaged[Thiscall]<Foo*, void>)(lpVtbl[0]))((Foo*)Unsafe.AsPointer(ref this));
        }
    }

    public unsafe partial struct Bar
    {
        public void** lpVtbl;

        public void BarMethod()
        {
            ((delegate* unmanaged[Thiscall]<Bar*, void>)(lpVtbl[0]))((Bar*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName(""struct Baz : Foo, Bar"")]
    public unsafe partial struct Baz
    {
        public void** lpVtbl;

        public void FooMethod()
        {
            ((delegate* unmanaged[Thiscall]<Baz*, void>)(lpVtbl[0]))((Baz*)Unsafe.AsPointer(ref this));
        }

        public void BarMethod()
        {
            ((delegate* unmanaged[Thiscall]<Baz*, void>)(lpVtbl[0]))((Baz*)Unsafe.AsPointer(ref this));
        }

        public void BazMethod()
        {
            ((delegate* unmanaged[Thiscall]<Baz*, void>)(lpVtbl[1]))((Baz*)Unsafe.AsPointer(ref this));
        }
    }
}
";

        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Unsupported cxx record declaration: 'multiple virtual bases'. Generated bindings may be incomplete.", "Line 11, Column 8 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(NonUnifyingInputContents, expectedOutputContents, expectedDiagnostics: expectedDiagnostics);
    }
}
