using System;

namespace ClangSharp
{
    public partial struct CXCursorSet : IDisposable
    {
        public static CXCursorSet Create() => clang.createCXCursorSet();

        public bool Contains(CXCursor cursor) => clang.CXCursorSet_contains(this, cursor) != 0;

        public void Dispose() => clang.disposeCXCursorSet(this);

        public bool Insert(CXCursor cursor) => clang.CXCursorSet_insert(this, cursor) != 0;
    }
}
