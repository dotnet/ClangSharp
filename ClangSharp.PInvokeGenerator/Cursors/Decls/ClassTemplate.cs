using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ClassTemplate : Decl
    {
        public ClassTemplate(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ClassTemplate);
        }
    }
}
