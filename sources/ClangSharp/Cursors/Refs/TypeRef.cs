using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TypeRef : Ref
    {
        public TypeRef(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_TypeRef);
        }
    }
}
