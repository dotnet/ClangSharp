using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ClassTemplateDecl : RedeclarableTemplateDecl
    {
        internal ClassTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ClassTemplate)
        {
        }
    }
}
