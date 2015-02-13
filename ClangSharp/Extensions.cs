namespace ClangSharp
{
    using System.Runtime.InteropServices;

    public static class Extensions
    {
        public static string String(this CXString cxString)
        {
            string retVal = Marshal.PtrToStringAnsi(cxString.data);
            Methods.clang_disposeString(cxString);
            return retVal;
        }
    }
}