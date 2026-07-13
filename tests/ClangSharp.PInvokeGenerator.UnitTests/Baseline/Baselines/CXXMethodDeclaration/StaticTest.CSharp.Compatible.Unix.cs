using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, EntryPoint = "_ZN8MyStruct12MyVoidMethodEv", ExactSpelling = true)]
        public static extern void MyVoidMethod();

        public static int MyInt32Method()
        {
            return 0;
        }

        public static void* MyVoidStarMethod()
        {
            return null;
        }
    }
}
