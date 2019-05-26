using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ClassDecl : Decl
    {
        public ClassDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ClassDecl);
        }
    }
}
