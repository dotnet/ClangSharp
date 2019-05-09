namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXIndexAction
    {
        public CXIndexAction(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
