using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TemplateTypeParmDecl : TypeDecl
    {
        public TemplateTypeParmDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TemplateTypeParameter);
        }
    }
}
