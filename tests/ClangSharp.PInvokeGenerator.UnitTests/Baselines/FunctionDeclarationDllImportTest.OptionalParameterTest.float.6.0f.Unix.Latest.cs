using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction(float value = 6.0f);
    }
}
