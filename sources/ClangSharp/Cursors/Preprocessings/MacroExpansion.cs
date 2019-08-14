using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MacroExpansion : Preprocessing
    {
        internal MacroExpansion(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MacroExpansion)
        {
        }

        public bool IsMacroBuiltIn => Handle.IsMacroBuiltIn;
    }
}
