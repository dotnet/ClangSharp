using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class TemplateRef : Ref
    {
        public TemplateRef(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TemplateRef);
        }
    }
}
