using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", ExactSpelling = true)]
        public static extern void MyFunction1(int value);

        [DllImport("ClangSharpPInvokeGenerator", ExactSpelling = true)]
        public static extern void MyFunction2(int value);
    }
}
