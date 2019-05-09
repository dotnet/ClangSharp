namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXVirtualFileOverlay
    {
        public CXVirtualFileOverlay(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
