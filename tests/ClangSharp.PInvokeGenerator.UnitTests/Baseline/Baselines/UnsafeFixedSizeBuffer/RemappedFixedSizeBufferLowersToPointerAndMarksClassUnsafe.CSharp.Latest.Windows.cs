using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("MyBuffer")] sbyte* value);

        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("MyBuffer")]
        public static extern sbyte* MyOtherFunction();
    }
}
