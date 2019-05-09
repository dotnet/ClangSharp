namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIndex
    {
        public CXIndex(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
