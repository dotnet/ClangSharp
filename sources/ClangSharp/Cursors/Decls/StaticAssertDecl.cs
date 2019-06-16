using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StaticAssertDecl : Decl
    {
        internal StaticAssertDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_StaticAssert)
        {
        }
    }
}
