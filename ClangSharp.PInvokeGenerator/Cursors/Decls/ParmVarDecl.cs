using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class ParmVarDecl : VarDecl
    {
        public ParmVarDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_ParmDecl);
        }
    }
}
