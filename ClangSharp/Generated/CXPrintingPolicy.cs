namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXPrintingPolicy
    {
        public CXPrintingPolicy(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
