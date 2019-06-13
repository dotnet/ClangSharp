using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXFile
    {
        public CXFile(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static explicit operator CXFile(void* value)
        {
            return new CXFile((IntPtr)value);
        }

        public static implicit operator void*(CXFile value)
        {
            return (void*)value.Pointer;
        }
    }
}
