using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MacroExpansion : PreprocessedEntity
    {
        internal MacroExpansion(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MacroExpansion)
        {
        }

        public bool IsMacroBuiltIn => Handle.IsMacroBuiltIn;
    }
}
