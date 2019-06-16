using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class InheritableAttr : Attr
    {
        internal InheritableAttr(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
