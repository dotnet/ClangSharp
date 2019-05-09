namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXCompilationDatabase
    {
        public CXCompilationDatabase(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
