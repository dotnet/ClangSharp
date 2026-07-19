using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void MyFunction([Optional, DefaultParameterValue(0)] HRESULT value);

        [NativeTypeName("#define S_OK ((HRESULT)0L)")]
        public const int S_OK = ((int)(0));
    }
}
