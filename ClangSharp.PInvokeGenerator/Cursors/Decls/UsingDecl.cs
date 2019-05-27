using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class UsingDecl : NamedDecl
    {
        public UsingDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_UsingDeclaration);
        }
    }
}
