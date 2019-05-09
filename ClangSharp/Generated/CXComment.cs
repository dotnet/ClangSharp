namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXComment
    {
        public IntPtr ASTNode;
        public CXTranslationUnit TranslationUnit;
    }
}
