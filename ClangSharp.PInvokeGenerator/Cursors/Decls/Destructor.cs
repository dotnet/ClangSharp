using System.Diagnostics;

namespace ClangSharp
{
    internal sealed class Destructor : Decl
    {
        public Destructor(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_Destructor);
        }
    }
}
