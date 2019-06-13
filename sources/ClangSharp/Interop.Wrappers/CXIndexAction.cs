using System;

namespace ClangSharp.Interop
{
    public partial struct CXIndexAction
    {
        public CXIndexAction(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
