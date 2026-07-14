using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public void** lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType(MyStruct* pThis, int objA, int objB);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType1(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType2(MyStruct* pThis, int obj);

        public int GetType(int objA, int objB)
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_GetType>((IntPtr)(lpVtbl[0]))(pThis, objA, objB);
            }
        }

        public new int GetType()
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_GetType1>((IntPtr)(lpVtbl[1]))(pThis);
            }
        }

        public int GetType(int obj)
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_GetType2>((IntPtr)(lpVtbl[2]))(pThis, obj);
            }
        }
    }
}
