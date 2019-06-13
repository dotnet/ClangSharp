using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXDiagnostic : IEquatable<CXDiagnostic>
    {
        public CXDiagnostic(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXDiagnostic(void* value) => new CXDiagnostic((IntPtr)value);

        public static implicit operator void*(CXDiagnostic value) => (void*)value.Handle;

        public static bool operator ==(CXDiagnostic left, CXDiagnostic right) => left.Handle == right.Handle;

        public static bool operator !=(CXDiagnostic left, CXDiagnostic right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXDiagnostic other) && Equals(other);

        public bool Equals(CXDiagnostic other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
