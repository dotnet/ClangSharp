using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnexposedAttr : Attr
    {
        public UnexposedAttr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnexposedAttr);
        }
    }
}
