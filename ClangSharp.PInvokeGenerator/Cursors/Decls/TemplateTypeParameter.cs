using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class TemplateTypeParameter : Decl
    {
        public TemplateTypeParameter(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TemplateTypeParameter);
        }
    }
}
