using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnionDecl : CXXRecordDecl
    {
        public UnionDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnionDecl);
        }
    }
}
