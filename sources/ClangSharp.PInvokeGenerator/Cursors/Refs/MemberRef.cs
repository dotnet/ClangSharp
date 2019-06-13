using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class MemberRef : Ref
    {
        public MemberRef(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_MemberRef);
        }
    }
}
