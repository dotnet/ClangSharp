using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public partial struct MyStruct
    {
        [NativeTypeName("decltype(&MyFunction)")]
        public IntPtr _callback;
    }

    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
