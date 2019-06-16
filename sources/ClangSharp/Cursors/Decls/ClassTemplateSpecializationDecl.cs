using ClangSharp.Interop;

namespace ClangSharp
{
    public class ClassTemplateSpecializationDecl : CXXRecordDecl
    {
        private protected ClassTemplateSpecializationDecl(CXCursor handle, CXCursorKind expectedKind) : base(handle, expectedKind)
        {
        }
    }
}
