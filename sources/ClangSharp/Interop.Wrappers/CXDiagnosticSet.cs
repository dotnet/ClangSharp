using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXDiagnosticSet
    {
        public CXDiagnosticSet(IntPtr pointer)
        {
            Pointer = pointer;
        }

        public IntPtr Pointer;

        public static explicit operator CXDiagnosticSet(void* value)
        {
            return new CXDiagnosticSet((IntPtr)value);
        }

        public static implicit operator void*(CXDiagnosticSet value)
        {
            return (void*)value.Pointer;
        }
    }
}
