using System.Runtime.CompilerServices;

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

    [NativeTypeName("struct C : Foo")]
    public unsafe partial struct C
    {
        public Foo Base;

        public void A()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(Base.lpVtbl[0]))((C*)Unsafe.AsPointer(ref this));
        }

        public void c()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(Base.lpVtbl[1]))((C*)Unsafe.AsPointer(ref this));
        }
    }
}
