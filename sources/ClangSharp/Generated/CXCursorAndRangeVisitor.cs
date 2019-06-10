using System;

namespace ClangSharp
{
    public unsafe partial struct CXCursorAndRangeVisitor
    {
        public void* context;

        [NativeTypeName("enum CXVisitorResult (*)(void*, CXCursor, CXSourceRange)")]
        public IntPtr visit;
    }
}
