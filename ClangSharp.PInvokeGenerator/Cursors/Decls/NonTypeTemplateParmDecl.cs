using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class NonTypeTemplateParmDecl : DeclaratorDecl
    {
        public NonTypeTemplateParmDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_NonTypeTemplateParameter);
        }
    }
}
