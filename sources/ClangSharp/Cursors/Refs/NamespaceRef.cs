using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NamespaceRef : Ref
    {
        public NamespaceRef(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_NamespaceRef);
        }
    }
}
