namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXClientData
    {
        public CXClientData(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
