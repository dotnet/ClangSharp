using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConditionalOperator : AbstractConditionalOperator
    {
        public ConditionalOperator(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ConditionalOperator);
        }
    }
}
