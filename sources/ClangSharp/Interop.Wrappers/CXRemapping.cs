using System;

namespace ClangSharp.Interop
{
    public partial struct CXRemapping
    {
        public CXRemapping(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
