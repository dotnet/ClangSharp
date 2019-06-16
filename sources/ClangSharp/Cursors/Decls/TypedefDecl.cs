using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypedefDecl : TypedefNameDecl
    {
        internal TypedefDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TypedefDecl)
        {
        }
    }
}
