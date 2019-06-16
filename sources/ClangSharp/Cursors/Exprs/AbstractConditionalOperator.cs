using ClangSharp.Interop;

namespace ClangSharp
{
    public class AbstractConditionalOperator : Expr
    {
        private protected AbstractConditionalOperator(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
