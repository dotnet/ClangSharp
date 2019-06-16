using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ClassTemplatePartialSpecializationDecl : ClassTemplateSpecializationDecl
    {
        internal ClassTemplatePartialSpecializationDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ClassTemplatePartialSpecialization)
        {
        }
    }
}
