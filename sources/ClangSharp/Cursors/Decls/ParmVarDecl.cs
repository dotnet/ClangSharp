using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ParmVarDecl : VarDecl
    {
        internal ParmVarDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ParmDecl)
        {
        }
    }
}
