using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStructA
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyMethod(MyStructA* pThis);

        public void MyMethod()
        {
            fixed (MyStructA* pThis = &this)
            {
                Marshal.GetDelegateForFunctionPointer<_MyMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }
        }
    }

    [NativeTypeName("struct MyStructB : MyStructA")]
    public unsafe partial struct MyStructB
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyMethod(MyStructB* pThis);

        public void MyMethod()
        {
            fixed (MyStructB* pThis = &this)
            {
                Marshal.GetDelegateForFunctionPointer<_MyMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }
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
