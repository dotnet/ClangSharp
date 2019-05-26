using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnexposedDecl : Decl
    {
        public UnexposedDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnexposedDecl);
        }
    }
}
