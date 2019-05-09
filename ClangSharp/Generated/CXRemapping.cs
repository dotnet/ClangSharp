namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXRemapping
    {
        public CXRemapping(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
