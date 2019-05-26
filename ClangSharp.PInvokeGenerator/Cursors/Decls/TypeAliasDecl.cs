using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TypeAliasDecl : Decl
    {
        public TypeAliasDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TypeAliasDecl);
        }
    }
}
