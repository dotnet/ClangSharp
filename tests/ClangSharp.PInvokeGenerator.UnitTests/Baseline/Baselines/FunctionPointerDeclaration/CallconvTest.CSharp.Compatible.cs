using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void Callback();

    public partial struct MyStruct
    {
        [NativeTypeName("Callback")]
        public IntPtr _callback;
    }
}
