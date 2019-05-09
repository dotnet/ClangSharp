namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXFile
    {
        public CXFile(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
