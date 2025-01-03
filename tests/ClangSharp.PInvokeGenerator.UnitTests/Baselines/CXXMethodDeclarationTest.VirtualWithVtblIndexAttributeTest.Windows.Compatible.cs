using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void _MyVoidMethod(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        [return: NativeTypeName("char")]
        public delegate sbyte _MyInt8Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _MyInt32Method(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate void* _MyVoidStarMethod(MyStruct* pThis);

        [VtblIndex(0)]
        public void MyVoidMethod()
        {
            fixed (MyStruct* pThis = &this)
            {
                Marshal.GetDelegateForFunctionPointer<_MyVoidMethod>((IntPtr)(lpVtbl[0]))(pThis);
            }
        }

        [VtblIndex(1)]
        [return: NativeTypeName("char")]
        public sbyte MyInt8Method()
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_MyInt8Method>((IntPtr)(lpVtbl[1]))(pThis);
            }
        }

        [VtblIndex(2)]
        public int MyInt32Method()
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_MyInt32Method>((IntPtr)(lpVtbl[2]))(pThis);
            }
        }

        [VtblIndex(3)]
        public void* MyVoidStarMethod()
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_MyVoidStarMethod>((IntPtr)(lpVtbl[3]))(pThis);
            }
        }
    }
}
