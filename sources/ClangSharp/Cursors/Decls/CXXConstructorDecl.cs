using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXConstructorDecl : CXXMethodDecl
    {
        public CXXConstructorDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_Constructor);
        }

        public bool IsConvertingConstructor => Handle.CXXConstructor_IsConvertingConstructor;

        public bool IsCopyConstructor => Handle.CXXConstructor_IsCopyConstructor;

        public bool IsDefaultConstructor => Handle.CXXConstructor_IsDefaultConstructor;

        public bool IsMoveConstructor => Handle.CXXConstructor_IsMoveConstructor;
    }
}
