using ClangSharp.Interop;

namespace ClangSharp
{
    internal class AbstractConditionalOperator : Expr
    {
        protected AbstractConditionalOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
