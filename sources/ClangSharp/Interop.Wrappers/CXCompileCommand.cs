using System;

namespace ClangSharp.Interop
{
    public partial struct CXCompileCommand
    {
        public CXCompileCommand(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
