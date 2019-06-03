using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class FunctionTemplateDecl : RedeclarableTemplateDecl
    {
        public FunctionTemplateDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_FunctionTemplate);
            Type = TranslationUnit.GetOrCreateType(Handle.Type, () => Type.Create(Handle.Type, TranslationUnit));
        }

        public Type Type { get; }
    }
}
