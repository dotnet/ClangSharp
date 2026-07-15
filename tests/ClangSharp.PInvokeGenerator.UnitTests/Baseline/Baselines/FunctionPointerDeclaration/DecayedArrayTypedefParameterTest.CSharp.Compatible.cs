using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int Callback([NativeTypeName("ARR")] ushort* szPRFHash);

    public partial struct MyStruct
    {
        [NativeTypeName("Callback")]
        public IntPtr _callback;
    }
}
