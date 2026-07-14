using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct Foo
    {
        public Vtbl* lpVtbl;

        public int x;

        public void A()
        {
            lpVtbl->A((Foo*)Unsafe.AsPointer(ref this));
        }

        public partial struct Vtbl
        {
            [NativeTypeName("void ()")]
            public delegate* unmanaged[Thiscall]<Foo*, void> A;
        }
    }

    [NativeTypeName("struct C : Foo")]
    public unsafe partial struct C
    {
        public Foo Base;

        public void A()
        {
            ((Vtbl*)Base.lpVtbl)->A((C*)Unsafe.AsPointer(ref this));
        }

        public void c()
        {
            ((Vtbl*)Base.lpVtbl)->c((C*)Unsafe.AsPointer(ref this));
        }

        public partial struct Vtbl
        {
            [NativeTypeName("void ()")]
            public delegate* unmanaged[Thiscall]<C*, void> A;

            [NativeTypeName("void ()")]
            public delegate* unmanaged[Thiscall]<C*, void> c;
        }
    }
}
