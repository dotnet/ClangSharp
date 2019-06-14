using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class PureAttr : Attr
    {
        public PureAttr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_PureAttr);
        }
    }
}
