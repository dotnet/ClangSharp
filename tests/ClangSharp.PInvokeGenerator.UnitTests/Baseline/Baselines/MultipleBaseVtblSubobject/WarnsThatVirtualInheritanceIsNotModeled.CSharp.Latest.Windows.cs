using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct A
    {
        public void** lpVtbl;

        public int ax;

        public void a()
        {
            ((delegate* unmanaged[Thiscall]<A*, void>)(lpVtbl[0]))((A*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct C : A")]
    public unsafe partial struct C
    {
        public A Base;

        public int cx;

        public void a()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(Base.lpVtbl[0]))((C*)Unsafe.AsPointer(ref this));
        }

        public void c()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(Base.lpVtbl[0]))((C*)Unsafe.AsPointer(ref this));
        }
    }
}
