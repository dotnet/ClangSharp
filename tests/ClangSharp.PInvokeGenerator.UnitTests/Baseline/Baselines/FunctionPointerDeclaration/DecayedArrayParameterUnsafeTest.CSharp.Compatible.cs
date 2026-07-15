using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public unsafe delegate int Callback([NativeTypeName("unsigned short[64]")] ushort* szPRFHash);

    public partial struct MyStruct
    {
        [NativeTypeName("Callback")]
        public IntPtr _callback;
    }
}
