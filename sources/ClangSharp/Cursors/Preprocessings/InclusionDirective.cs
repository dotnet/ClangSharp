using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class InclusionDirective : PreprocessingDirective
    {
        internal InclusionDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_InclusionDirective)
        {
        }
    }
}
