using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXFinalAttr : Attr
    {
        public CXXFinalAttr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXFinalAttr);
        }
    }
}
