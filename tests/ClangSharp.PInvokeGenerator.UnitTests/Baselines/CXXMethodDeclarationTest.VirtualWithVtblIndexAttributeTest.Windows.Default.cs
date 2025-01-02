using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public void** lpVtbl;

        [VtblIndex(0)]
        public void MyVoidMethod()
        {
            ((delegate* unmanaged[Thiscall]<MyStruct*, void>)(lpVtbl[0]))((MyStruct*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(1)]
        [return: NativeTypeName("char")]
        public sbyte MyInt8Method()
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, sbyte>)(lpVtbl[1]))((MyStruct*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(2)]
        public int MyInt32Method()
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, int>)(lpVtbl[2]))((MyStruct*)Unsafe.AsPointer(ref this));
        }

        [VtblIndex(3)]
        public void* MyVoidStarMethod()
        {
            return ((delegate* unmanaged[Thiscall]<MyStruct*, void*>)(lpVtbl[3]))((MyStruct*)Unsafe.AsPointer(ref this));
        }
    }
}
