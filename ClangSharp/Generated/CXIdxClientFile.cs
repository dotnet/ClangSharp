using System;

namespace ClangSharp
{
    public partial struct CXIdxClientFile
    {
        public CXIdxClientFile(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
