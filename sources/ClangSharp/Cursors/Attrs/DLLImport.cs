using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DLLImport : Attr
    {
        public DLLImport(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_DLLImport);
        }
    }
}
