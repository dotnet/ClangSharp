namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXCursorSet
    {
        public CXCursorSet(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
