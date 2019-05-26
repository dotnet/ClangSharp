using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class FunctionTemplate : Decl
    {
        public FunctionTemplate(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FunctionTemplate);
        }
    }
}
