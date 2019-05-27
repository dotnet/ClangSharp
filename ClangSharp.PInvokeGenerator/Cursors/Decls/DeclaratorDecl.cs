using System;

namespace ClangSharp
{
    internal class DeclaratorDecl : ValueDecl
    {
        protected DeclaratorDecl(CXCursor handle, Cursor parent) : base(handle, parent)
        {
        }
    }
}
