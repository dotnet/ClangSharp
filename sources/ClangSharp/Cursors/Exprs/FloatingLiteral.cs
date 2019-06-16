using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FloatingLiteral : Expr
    {
        internal FloatingLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FloatingLiteral)
        {
        }
    }
}
