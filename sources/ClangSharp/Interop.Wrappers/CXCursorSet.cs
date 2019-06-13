using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCursorSet
    {
        public CXCursorSet(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static implicit operator CXCursorSet(CXCursorSetImpl* value)
        {
            return new CXCursorSet((IntPtr)value);
        }

        public static implicit operator CXCursorSetImpl*(CXCursorSet value)
        {
            return (CXCursorSetImpl*)value.Pointer;
        }
    }
}
