namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXCompileCommand
    {
        public CXCompileCommand(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
