namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXCompileCommands
    {
        public CXCompileCommands(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
