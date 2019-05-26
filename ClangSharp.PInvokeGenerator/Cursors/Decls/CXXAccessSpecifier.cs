using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class CXXAccessSpecifier : Decl
    {
        public CXXAccessSpecifier(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXAccessSpecifier);
        }
    }
}
