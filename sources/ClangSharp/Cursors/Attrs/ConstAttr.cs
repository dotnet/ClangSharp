using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConstAttr : Attr
    {
        public ConstAttr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ConstAttr);
        }
    }
}
