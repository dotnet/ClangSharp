using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class FunctionTemplateDecl : RedeclarableTemplateDecl
    {
        internal FunctionTemplateDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_FunctionTemplate)
        {
        }
    }
}
