using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UsingDeclaration : Decl
    {
        public UsingDeclaration(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UsingDeclaration);
        }
    }
}
