using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class OverloadedDeclRef : Ref
    {
        public OverloadedDeclRef(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_OverloadedDeclRef);
        }
    }
}
