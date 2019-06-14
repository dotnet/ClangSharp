using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXBaseSpecifier : Ref
    {
        public CXXBaseSpecifier(CXCursor handle, Cursor parent) : base(handle, parent)
        {
            Debug.Assert(handle.Kind == CXCursorKind.CXCursor_CXXBaseSpecifier);
        }

        public CX_CXXAccessSpecifier AccessSpecifier => Handle.CXXAccessSpecifier;

        public bool IsVirtual => Handle.IsVirtualBase;
    }
}
