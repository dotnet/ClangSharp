using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CharacterLiteral : Expr
    {
        internal CharacterLiteral(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CharacterLiteral)
        {
        }
    }
}
