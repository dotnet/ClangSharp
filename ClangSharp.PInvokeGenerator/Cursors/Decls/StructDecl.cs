using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class StructDecl : CXXRecordDecl
    {
        public StructDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_StructDecl);
        }
    }
}
