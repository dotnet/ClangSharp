using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypeAliasDecl : TypedefNameDecl
    {
        internal TypeAliasDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TypeAliasDecl)
        {
        }
    }
}
