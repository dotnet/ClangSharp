using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AccessSpecDecl : Decl
    {
        public AccessSpecDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXAccessSpecifier);
        }
    }
}
