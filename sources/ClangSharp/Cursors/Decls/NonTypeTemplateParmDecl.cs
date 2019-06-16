using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class NonTypeTemplateParmDecl : DeclaratorDecl, ITemplateParmPosition
    {
        internal NonTypeTemplateParmDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_NonTypeTemplateParameter)
        {
        }
    }
}
