using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    public delegate void PFoo(int param0);

    public partial struct Callbacks
    {
        [NativeTypeName("ApiPFoo *")]
        public IntPtr Foo;
    }
}
