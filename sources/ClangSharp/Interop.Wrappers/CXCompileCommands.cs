using System;

namespace ClangSharp.Interop
{
    public partial struct CXCompileCommands
    {
        public CXCompileCommands(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
