using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public Vtbl* lpVtbl;

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType(MyStruct* pThis, int obj);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType1(MyStruct* pThis);

        [UnmanagedFunctionPointer(CallingConvention.ThisCall)]
        public delegate int _GetType2(MyStruct* pThis, int objA, int objB);

        public int GetType(int obj)
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_GetType>(lpVtbl->GetType)(pThis, obj);
            }
        }

        public new int GetType()
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_GetType1>(lpVtbl->GetType1)(pThis);
            }
        }

        public int GetType(int objA, int objB)
        {
            fixed (MyStruct* pThis = &this)
            {
                return Marshal.GetDelegateForFunctionPointer<_GetType2>(lpVtbl->GetType2)(pThis, objA, objB);
            }
        }

        public partial struct Vtbl
        {
            [NativeTypeName("int (int)")]
            public new IntPtr GetType;

            [NativeTypeName("int ()")]
            public IntPtr GetType1;

            [NativeTypeName("int (int, int)")]
            public IntPtr GetType2;
        }
    }
}
