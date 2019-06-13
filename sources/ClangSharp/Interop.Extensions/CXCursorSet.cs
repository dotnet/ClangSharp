using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCursorSet : IDisposable
    {
        public static CXCursorSet Create() => clang.createCXCursorSet();

        public bool Contains(CXCursor cursor) => clang.CXCursorSet_contains(this, cursor) != 0;

        public void Dispose()
        {
            if (Pointer != IntPtr.Zero)
            {
                clang.disposeCXCursorSet(this);
                Pointer = IntPtr.Zero;
            }
        }

        public bool Insert(CXCursor cursor) => clang.CXCursorSet_insert(this, cursor) != 0;
    }
}
