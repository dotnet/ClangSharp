using System;

namespace ClangSharp.Interop
{
    public partial struct CXCompilationDatabase
    {
        public CXCompilationDatabase(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
