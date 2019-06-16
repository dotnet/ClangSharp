using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXDestructorDecl : CXXMethodDecl
    {
        internal CXXDestructorDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Destructor)
        {
        }
    }
}
