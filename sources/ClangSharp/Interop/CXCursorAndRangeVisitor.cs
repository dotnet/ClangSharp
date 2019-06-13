using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCursorAndRangeVisitor
    {
        [NativeTypeName("void *")]
        public void* context;

        [NativeTypeName("enum CXVisitorResult (*)(void *, CXCursor, CXSourceRange)")]
        public IntPtr visit;
    }
}
