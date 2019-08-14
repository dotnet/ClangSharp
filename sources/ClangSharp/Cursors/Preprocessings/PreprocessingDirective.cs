using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PreprocessingDirective : Preprocessing
    {
        internal PreprocessingDirective(CXCursor handle) : base(handle, CXCursorKind.CXCursor_PreprocessingDirective)
        {
        }
    }
}
