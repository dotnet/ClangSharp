using System;

namespace ClangSharp
{
    public partial struct CXClientData
    {
        public CXClientData(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
