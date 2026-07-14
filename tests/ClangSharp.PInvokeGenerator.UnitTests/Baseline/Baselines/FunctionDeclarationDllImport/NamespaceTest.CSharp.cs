using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "?MyFunction@MyNamespace@@YAXXZ", ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
