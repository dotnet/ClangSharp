using System.Diagnostics;

namespace ClangSharp
{
    internal class StaticAssert : Decl
    {
        public StaticAssert(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_StaticAssert);
        }
    }
}
