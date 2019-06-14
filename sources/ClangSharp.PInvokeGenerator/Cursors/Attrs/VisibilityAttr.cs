using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class VisibilityAttr : Attr
    {
        public VisibilityAttr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_VisibilityAttr);
        }
    }
}
