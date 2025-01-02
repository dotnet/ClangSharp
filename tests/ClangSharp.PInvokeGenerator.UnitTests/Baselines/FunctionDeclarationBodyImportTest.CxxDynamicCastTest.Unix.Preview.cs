using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStructA
    {
        public void** lpVtbl;

        public void MyMethod()
        {
            ((delegate* unmanaged[Thiscall]<MyStructA*, void>)(lpVtbl[0]))((MyStructA*)Unsafe.AsPointer(ref this));
        }
    }

    [NativeTypeName("struct MyStructB : MyStructA")]
    public unsafe partial struct MyStructB
    {
        public void** lpVtbl;

        public void MyMethod()
        {
            ((delegate* unmanaged[Thiscall]<MyStructB*, void>)(lpVtbl[0]))((MyStructB*)Unsafe.AsPointer(ref this));
        }
    }

    public static unsafe partial class Methods
    {
        public static MyStructB* MyFunction(MyStructA* input)
        {
            return (MyStructB*)(input);
        }
    }
}
