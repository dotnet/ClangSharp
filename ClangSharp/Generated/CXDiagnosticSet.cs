namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXDiagnosticSet
    {
        public CXDiagnosticSet(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
