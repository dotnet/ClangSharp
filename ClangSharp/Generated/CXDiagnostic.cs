namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXDiagnostic
    {
        public CXDiagnostic(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
