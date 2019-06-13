using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXModule
    {
        public CXModule(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static explicit operator CXModule(void* value)
        {
            return new CXModule((IntPtr)value);
        }

        public static implicit operator void*(CXModule value)
        {
            return (void*)value.Pointer;
        }
    }
}
