using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MacroDefinition : Preprocessing
    {
        internal MacroDefinition(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MacroDefinition)
        {
        }

        public bool IsMacroFunctionLike => Handle.IsMacroFunctionLike;

        public bool IsMacroBuiltIn => Handle.IsMacroBuiltIn;
    }
}
