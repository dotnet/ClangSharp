using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StringLiteral : Expr
    {
        internal StringLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_StringLiteral)
        {
        }
    }
}
