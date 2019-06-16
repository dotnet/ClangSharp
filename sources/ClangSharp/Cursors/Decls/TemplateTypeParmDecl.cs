using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateTypeParmDecl : TypeDecl
    {
        internal TemplateTypeParmDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_TemplateTypeParameter)
        {
        }
    }
}
