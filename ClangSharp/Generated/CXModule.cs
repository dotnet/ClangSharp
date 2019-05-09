namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXModule
    {
        public CXModule(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
