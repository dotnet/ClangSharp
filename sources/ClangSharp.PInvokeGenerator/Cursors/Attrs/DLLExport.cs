using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class DLLExport : Attr
    {
        public DLLExport(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DLLExport);
        }
    }
}
