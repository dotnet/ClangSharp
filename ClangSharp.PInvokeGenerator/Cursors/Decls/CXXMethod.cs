using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXMethod : Decl
    {
        public CXXMethod(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXMethod);
        }
    }
}
