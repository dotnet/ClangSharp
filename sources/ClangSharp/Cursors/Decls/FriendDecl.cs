using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FriendDecl : Decl
    {
        internal FriendDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FriendDecl)
        {
        }
    }
}
