using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class ConstAttr : Attr
    {
        public ConstAttr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ConstAttr);
        }
    }
}
