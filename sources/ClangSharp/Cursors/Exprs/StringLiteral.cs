using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StringLiteral : Expr
    {
        public StringLiteral(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_StringLiteral);
        }
    }
}
