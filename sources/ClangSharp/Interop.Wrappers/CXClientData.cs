using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXClientData
    {
        public CXClientData(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static explicit operator CXClientData(void* value)
        {
            return new CXClientData((IntPtr)value);
        }

        public static implicit operator void*(CXClientData value)
        {
            return (void*)value.Pointer;
        }
    }
}
