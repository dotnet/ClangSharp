using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public void** lpVtbl;

        public void MyVoidMethod()
        {
            ((delegate* unmanaged[Thiscall]<MyStruct*, void>)(lpVtbl[0]))((MyStruct*)Unsafe.AsPointer(ref this));
        }

        [return: NativeTypeName("char")]
        public sbyte MyInt8Method()
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, sbyte>)(lpVtbl[1]))((MyStruct*)Unsafe.AsPointer(ref this));
        }

        public int MyInt32Method()
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, int>)(lpVtbl[2]))((MyStruct*)Unsafe.AsPointer(ref this));
        }

        public void* MyVoidStarMethod()
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, void*>)(lpVtbl[3]))((MyStruct*)Unsafe.AsPointer(ref this));
        }
    }
}
