using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static unsafe partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([NativeTypeName("void (*)()")] delegate* unmanaged[Cdecl]<void> callback);
    }
}
