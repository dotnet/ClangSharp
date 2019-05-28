using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class FriendDecl : Decl
    {
        public FriendDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FriendDecl);
        }
    }
}
