using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXEvalResult
    {
        public CXEvalResult(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static explicit operator CXEvalResult(void* value)
        {
            return new CXEvalResult((IntPtr)value);
        }

        public static implicit operator void*(CXEvalResult value)
        {
            return (void*)value.Pointer;
        }
    }
}
