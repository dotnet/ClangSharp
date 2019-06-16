using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AccessSpecDecl : Decl
    {
        internal AccessSpecDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXAccessSpecifier)
        {
        }
    }
}
