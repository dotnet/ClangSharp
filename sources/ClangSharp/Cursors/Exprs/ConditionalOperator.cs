using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConditionalOperator : AbstractConditionalOperator
    {
        internal ConditionalOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ConditionalOperator)
        {
        }
    }
}
