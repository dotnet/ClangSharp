using ClangSharp.Interop;

namespace ClangSharp
{
    public class DeclaratorDecl : ValueDecl
    {
        protected DeclaratorDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
