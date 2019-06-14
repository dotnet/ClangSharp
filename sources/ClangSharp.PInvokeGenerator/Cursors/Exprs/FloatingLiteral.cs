using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class FloatingLiteral : Expr
    {
        public FloatingLiteral(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FloatingLiteral);
        }
    }
}
