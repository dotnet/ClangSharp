using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class InclusionDirective : Preprocessing
    {
        internal InclusionDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_InclusionDirective)
        {
        }
    }
}
