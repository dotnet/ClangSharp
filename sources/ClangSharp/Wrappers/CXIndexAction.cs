using System;

namespace ClangSharp
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
