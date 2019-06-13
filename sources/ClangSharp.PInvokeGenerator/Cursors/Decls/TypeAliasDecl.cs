using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class TypeAliasDecl : TypedefNameDecl
    {
        public TypeAliasDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TypeAliasDecl);
        }
    }
}
