// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Threading.Tasks;
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
public sealed class MultipleBaseVtblSubobjectTest : PInvokeGeneratorTest
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
    public Task EmitsSecondaryBaseAsSubobject()
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

        public Bar Base2;

        public void Dispose()
        {
            ((delegate* unmanaged[Thiscall]<Baz*, void>)(lpVtbl[0]))((Baz*)Unsafe.AsPointer(ref this));
        }
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(InputContents, expectedOutputContents);
    }

    [Test]
    public Task EmitsSecondaryBaseAsSubobjectWithExplicitVtbls()
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

        public Bar Base2;

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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(InputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls);
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

        public Bar Base2;

        public void FooMethod()
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

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(NonUnifyingInputContents, expectedOutputContents);
    }

    [Test]
    public Task InheritsPrimaryBaseMarkerInterfaceOnly()
    {
        var expectedOutputContents = @"using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo : Foo.Interface
    {
        public Vtbl<Foo>* lpVtbl;

        public void FooMethod()
        {
            lpVtbl->FooMethod((Foo*)Unsafe.AsPointer(ref this));
        }

        public interface Interface
        {
            void FooMethod();
        }

        public partial struct Vtbl<TSelf>
            where TSelf : unmanaged, Interface
        {
            [NativeTypeName(""void ()"")]
            public delegate* unmanaged[Thiscall]<TSelf*, void> FooMethod;
        }
    }

    public unsafe partial struct Bar : Bar.Interface
    {
        public Vtbl<Bar>* lpVtbl;

        public void BarMethod()
        {
            lpVtbl->BarMethod((Bar*)Unsafe.AsPointer(ref this));
        }

        public interface Interface
        {
            void BarMethod();
        }

        public partial struct Vtbl<TSelf>
            where TSelf : unmanaged, Interface
        {
            [NativeTypeName(""void ()"")]
            public delegate* unmanaged[Thiscall]<TSelf*, void> BarMethod;
        }
    }

    [NativeTypeName(""struct Baz : Foo, Bar"")]
    public unsafe partial struct Baz : Baz.Interface
    {
        public Vtbl<Baz>* lpVtbl;

        public Bar Base2;

        public void FooMethod()
        {
            lpVtbl->FooMethod((Baz*)Unsafe.AsPointer(ref this));
        }

        public void BazMethod()
        {
            lpVtbl->BazMethod((Baz*)Unsafe.AsPointer(ref this));
        }

        public interface Interface : Foo.Interface
        {
            void BazMethod();
        }

        public partial struct Vtbl<TSelf>
            where TSelf : unmanaged, Interface
        {
            [NativeTypeName(""void ()"")]
            public delegate* unmanaged[Thiscall]<TSelf*, void> FooMethod;

            [NativeTypeName(""void ()"")]
            public delegate* unmanaged[Thiscall]<TSelf*, void> BazMethod;
        }
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(NonUnifyingInputContents, expectedOutputContents, PInvokeGeneratorConfigurationOptions.GenerateExplicitVtbls | PInvokeGeneratorConfigurationOptions.GenerateMarkerInterfaces);
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
        var expectedOutputContents = @"using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo
    {
        public void** lpVtbl;

        public int x;

        public void A()
        {
            ((delegate* unmanaged[Thiscall]<Foo*, void>)(lpVtbl[0]))((Foo*)Unsafe.AsPointer(ref this));
        }
    }

    public unsafe partial struct Bar
    {
        public void** lpVtbl;

        public void B()
        {
            ((delegate* unmanaged[Thiscall]<Bar*, void>)(lpVtbl[0]))((Bar*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName(""struct Baz : Foo, Bar"")]
    public partial struct Baz
    {
        public Foo Base1;

        public Bar Base2;
    }
}
";

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(FieldBearingPrimaryInputContents, expectedOutputContents);
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
        var expectedOutputContents = @"using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct A
    {
        public void** lpVtbl;

        public void a()
        {
            ((delegate* unmanaged[Thiscall]<A*, void>)(lpVtbl[0]))((A*)Unsafe.AsPointer(ref this));
        }
    }

    public unsafe partial struct B
    {
        public void** lpVtbl;

        public void b()
        {
            ((delegate* unmanaged[Thiscall]<B*, void>)(lpVtbl[0]))((B*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName(""struct C : A, B"")]
    public unsafe partial struct C
    {
        public void** lpVtbl;

        public B Base2;

        public void a()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(lpVtbl[0]))((C*)Unsafe.AsPointer(ref this));
        }

        public void c()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(lpVtbl[1]))((C*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName(""struct D : C"")]
    public unsafe partial struct D
    {
        public void** lpVtbl;

        public void a()
        {
            ((delegate* unmanaged[Thiscall]<D*, void>)(lpVtbl[0]))((D*)Unsafe.AsPointer(ref this));
        }

        public void b()
        {
            ((delegate* unmanaged[Thiscall]<D*, void>)(lpVtbl[0]))((D*)Unsafe.AsPointer(ref this));
        }

        public void c()
        {
            ((delegate* unmanaged[Thiscall]<D*, void>)(lpVtbl[1]))((D*)Unsafe.AsPointer(ref this));
        }

        public void d()
        {
            ((delegate* unmanaged[Thiscall]<D*, void>)(lpVtbl[2]))((D*)Unsafe.AsPointer(ref this));
        }
    }
}
";

        var expectedDiagnostics = new[] {
            new Diagnostic(DiagnosticLevel.Warning, "Unsupported cxx record declaration: 'nested multiple virtual bases'. Generated bindings for D may be incomplete.", "Line 16, Column 8 in ClangUnsavedFile.h")
        };

        return ValidateGeneratedCSharpLatestWindowsBindingsAsync(NestedMultipleInheritanceInputContents, expectedOutputContents, expectedDiagnostics: expectedDiagnostics);
    }
}
