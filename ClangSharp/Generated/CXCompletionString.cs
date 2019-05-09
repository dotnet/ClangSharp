namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXCompletionString
    {
        public CXCompletionString(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
