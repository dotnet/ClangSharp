using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompletionString
    {
        public CXCompletionString(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static explicit operator CXCompletionString(void* value)
        {
            return new CXCompletionString((IntPtr)value);
        }

        public static implicit operator void*(CXCompletionString value)
        {
            return (void*)value.Pointer;
        }
    }
}
