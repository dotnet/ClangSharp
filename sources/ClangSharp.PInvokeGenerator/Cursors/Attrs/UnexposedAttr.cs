using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UnexposedAttr : Attr
    {
        public UnexposedAttr(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UnexposedAttr);
        }
    }
}
