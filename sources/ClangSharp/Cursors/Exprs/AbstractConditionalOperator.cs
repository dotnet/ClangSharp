using ClangSharp.Interop;

namespace ClangSharp
{
    public class AbstractConditionalOperator : Expr
    {
        protected AbstractConditionalOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
