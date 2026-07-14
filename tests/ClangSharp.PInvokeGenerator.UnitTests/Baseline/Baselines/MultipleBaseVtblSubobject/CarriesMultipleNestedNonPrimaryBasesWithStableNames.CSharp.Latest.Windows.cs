using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct A
    {
        public void** lpVtbl;

        public void AMethod()
        {
            ((delegate* unmanaged[Thiscall]<A*, void>)(lpVtbl[0]))((A*)Unsafe.AsPointer(ref this));
        }
    }

    public unsafe partial struct B
    {
        public void** lpVtbl;

        public void BMethod()
        {
            ((delegate* unmanaged[Thiscall]<B*, void>)(lpVtbl[0]))((B*)Unsafe.AsPointer(ref this));
        }
    }

    public unsafe partial struct E
    {
        public void** lpVtbl;

        public void EMethod()
        {
            ((delegate* unmanaged[Thiscall]<E*, void>)(lpVtbl[0]))((E*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct C : A, B, E")]
    public unsafe partial struct C
    {
        public void** lpVtbl;

        public B Base2;

        public E Base3;

        public void AMethod()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(lpVtbl[0]))((C*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct D : C")]
    public unsafe partial struct D
    {
        public void** lpVtbl;

        public B Base2;

        public E Base3;

        public void AMethod()
        {
            ((delegate* unmanaged[Thiscall]<D*, void>)(lpVtbl[0]))((D*)Unsafe.AsPointer(ref this));
        }
    }
}
