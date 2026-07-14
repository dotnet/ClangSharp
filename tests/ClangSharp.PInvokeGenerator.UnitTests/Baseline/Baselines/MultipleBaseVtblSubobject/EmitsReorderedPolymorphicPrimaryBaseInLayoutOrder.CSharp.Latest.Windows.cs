using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct Data
    {
        public int dx;
    }

    public unsafe partial struct P
    {
        public void** lpVtbl;

        public int pp;

        public void v()
        {
            ((delegate* unmanaged[Thiscall]<P*, void>)(lpVtbl[0]))((P*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct C : Data, P")]
    public unsafe partial struct C
    {
        public P Base2;

        public Data Base1;

        public int cx;

        public void v()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(Base2.lpVtbl[0]))((C*)Unsafe.AsPointer(ref this));
        }

        public void c()
        {
            ((delegate* unmanaged[Thiscall]<C*, void>)(Base2.lpVtbl[1]))((C*)Unsafe.AsPointer(ref this));
        }
    }
}
