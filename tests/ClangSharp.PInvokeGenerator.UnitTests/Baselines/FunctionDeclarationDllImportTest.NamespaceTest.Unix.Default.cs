using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN11MyNamespace10MyFunctionEv", ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
