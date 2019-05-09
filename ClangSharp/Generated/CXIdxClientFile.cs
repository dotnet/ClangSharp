namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIdxClientFile
    {
        public CXIdxClientFile(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
