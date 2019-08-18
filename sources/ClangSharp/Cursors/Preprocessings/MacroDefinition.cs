using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MacroDefinitionRecord : PreprocessingDirective
    {
        internal MacroDefinitionRecord(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MacroDefinition)
        {
        }

        public bool IsMacroFunctionLike => Handle.IsMacroFunctionLike;

        public bool IsMacroBuiltIn => Handle.IsMacroBuiltIn;
    }
}
