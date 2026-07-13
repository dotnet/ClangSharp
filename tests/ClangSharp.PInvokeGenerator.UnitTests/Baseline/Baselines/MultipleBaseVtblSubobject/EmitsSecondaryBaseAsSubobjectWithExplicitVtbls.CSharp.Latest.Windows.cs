using System.Runtime.CompilerServices;

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
            [NativeTypeName("void () noexcept")]
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
            [NativeTypeName("void () noexcept")]
            public delegate* unmanaged[Thiscall]<Bar*, void> Dispose;
        }
    }

    [NativeTypeName("struct Baz : Foo, Bar")]
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
            [NativeTypeName("void () noexcept")]
            public delegate* unmanaged[Thiscall]<Baz*, void> Dispose;
        }
    }
}
