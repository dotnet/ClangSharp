using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("unsigned char")] byte value = 0);
    }
}
