using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Callback0();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [Obsolete]
    public delegate void Callback1();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    [Obsolete("This is obsolete.")]
    public delegate void Callback2();

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void Callback3();

    public partial struct MyStruct0
    {
        [NativeTypeName("Callback0")]
        public IntPtr _callback;
    }

    public partial struct MyStruct1
    {
        [NativeTypeName("Callback1")]
        [Obsolete]
        public IntPtr _callback;
    }

    public partial struct MyStruct2
    {
        [NativeTypeName("Callback2")]
        [Obsolete("This is obsolete.")]
        public IntPtr _callback;
    }

    public partial struct MyStruct3
    {
        [NativeTypeName("Callback3")]
        public IntPtr _callback;
    }
}
