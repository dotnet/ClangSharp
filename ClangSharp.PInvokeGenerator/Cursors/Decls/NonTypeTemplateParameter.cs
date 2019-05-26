using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class NonTypeTemplateParameter : Decl
    {
        public NonTypeTemplateParameter(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_NonTypeTemplateParameter);
        }
    }
}
