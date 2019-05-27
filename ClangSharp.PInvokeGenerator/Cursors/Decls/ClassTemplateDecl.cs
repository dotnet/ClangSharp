using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ClassTemplateDecl : RedeclarableTemplateDecl
    {
        public ClassTemplateDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ClassTemplate);
        }
    }
}
