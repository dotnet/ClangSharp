using System.Runtime.CompilerServices;

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

    [NativeTypeName("struct C : A, B")]
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

    [NativeTypeName("struct D : C")]
    public unsafe partial struct D
    {
        public void** lpVtbl;

        public B Base2;

        public void a()
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
