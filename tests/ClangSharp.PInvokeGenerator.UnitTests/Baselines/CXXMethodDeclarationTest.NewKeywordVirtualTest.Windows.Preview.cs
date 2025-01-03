using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public void** lpVtbl;

        public int GetType(int objA, int objB)
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, int, int, int>)(lpVtbl[0]))((MyStruct*)Unsafe.AsPointer(ref this), objA, objB);
        }

        public new int GetType()
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, int>)(lpVtbl[1]))((MyStruct*)Unsafe.AsPointer(ref this));
        }

        public int GetType(int obj)
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, int, int>)(lpVtbl[2]))((MyStruct*)Unsafe.AsPointer(ref this), obj);
        }
    }
}
