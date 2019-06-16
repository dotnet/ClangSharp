using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateTemplateParmDecl : TemplateDecl, ITemplateParmPosition
    {
        internal TemplateTemplateParmDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TemplateTemplateParameter)
        {
        }
    }
}
