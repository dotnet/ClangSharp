using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        public void* MyVoidStarMethod()
        {
            return null;
        }
    }

    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction();
    }
}
