using System;

namespace ClangSharp
{
    public unsafe partial struct CXDiagnostic
    {
        public CXDiagnostic(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static explicit operator CXDiagnostic(void* value)
        {
            return new CXDiagnostic((IntPtr)value);
        }

        public static implicit operator void*(CXDiagnostic value)
        {
            return (void*)value.Pointer;
        }
    }
}
