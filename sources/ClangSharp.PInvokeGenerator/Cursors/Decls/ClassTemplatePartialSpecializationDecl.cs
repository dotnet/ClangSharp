using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    internal sealed class ClassTemplatePartialSpecializationDecl : ClassTemplateSpecializationDecl
    {
        public ClassTemplatePartialSpecializationDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ClassTemplatePartialSpecialization);
        }
    }
}
