using ClangSharp.Interop;

namespace ClangSharp
{
    public class PreprocessingDirective : PreprocessedEntity
    {
        internal PreprocessingDirective(CXCursor handle) : this(handle, CXCursorKind.CXCursor_PreprocessingDirective)
        {
        }
        private protected PreprocessingDirective(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
