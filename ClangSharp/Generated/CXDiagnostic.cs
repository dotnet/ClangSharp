using System;

namespace ClangSharp
{
    public partial struct CXDiagnostic
    {
        public CXDiagnostic(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
