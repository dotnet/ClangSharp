using ClangSharp.Interop;

namespace ClangSharp
{
    public class TemplateDecl : NamedDecl
    {
        private protected TemplateDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
