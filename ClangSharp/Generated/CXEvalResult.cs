namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXEvalResult
    {
        public CXEvalResult(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
