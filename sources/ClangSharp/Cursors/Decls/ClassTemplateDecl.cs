using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ClassTemplateDecl : RedeclarableTemplateDecl
    {
        public ClassTemplateDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ClassTemplate);
        }

        public string DisplayName => Handle.DisplayName.ToString();
    }
}
