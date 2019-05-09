namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXTranslationUnit
    {
        public CXTranslationUnit(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;
    }
}
