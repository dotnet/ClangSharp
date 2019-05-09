namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXTargetInfo
    {
        public CXTargetInfo(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
