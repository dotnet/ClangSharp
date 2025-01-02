using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    public static partial class Methods
    {
        [DllImport("ClangSharpPInvokeGenerator", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [SourceLocation("ClangUnsavedFile.h", 1, 17)]
        public static extern void MyFunction([SourceLocation("ClangUnsavedFile.h", 1, 34)] float value);
    }
}
