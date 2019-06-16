using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXConstructorDecl : CXXMethodDecl
    {
        internal CXXConstructorDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Constructor)
        {
        }

        public bool IsConvertingConstructor => Handle.CXXConstructor_IsConvertingConstructor;

        public bool IsCopyConstructor => Handle.CXXConstructor_IsCopyConstructor;

        public bool IsDefaultConstructor => Handle.CXXConstructor_IsDefaultConstructor;

        public bool IsMoveConstructor => Handle.CXXConstructor_IsMoveConstructor;
    }
}
