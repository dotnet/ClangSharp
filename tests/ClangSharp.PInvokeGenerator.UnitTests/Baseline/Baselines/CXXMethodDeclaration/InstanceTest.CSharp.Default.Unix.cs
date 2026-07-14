using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public unsafe partial struct MyStruct
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.ThisCall, EntryPoint = "_ZN8MyStruct12MyVoidMethodEv", ExactSpelling = true)]
        public static extern void MyVoidMethod(MyStruct* pThis);

        public int MyInt32Method()
        {
            return 0;
        }

        public void* MyVoidStarMethod()
        {
            return null;
        }
    }
}
