using System;

namespace ClangSharp
{
    public unsafe partial struct CXIndex
    {
        public CXIndex(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static explicit operator CXIndex(void* value)
        {
            return new CXIndex((IntPtr)value);
        }

        public static implicit operator void*(CXIndex value)
        {
            return (void*)value.Pointer;
        }
    }
}
