using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ClassTemplatePartialSpecialization : Decl
    {
        public ClassTemplatePartialSpecialization(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ClassTemplatePartialSpecialization);
        }
    }
}
