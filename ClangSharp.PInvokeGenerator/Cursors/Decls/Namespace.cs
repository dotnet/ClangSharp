using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class Namespace : Decl
    {
        public Namespace(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_Namespace);
        }
    }
}
