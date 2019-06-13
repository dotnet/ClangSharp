using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class StaticAssertDecl : Decl
    {
        public StaticAssertDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_StaticAssert);
        }
    }
}
