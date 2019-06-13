using System;

namespace ClangSharp.Interop
{
    public partial struct CXPrintingPolicy
    {
        public CXPrintingPolicy(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
