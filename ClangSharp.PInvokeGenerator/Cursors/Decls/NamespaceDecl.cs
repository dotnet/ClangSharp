using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class NamespaceDecl : NamedDecl
    {
        public NamespaceDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_Namespace);
        }

        public bool IsAnonymous => Handle.IsAnonymous;
    }
}
