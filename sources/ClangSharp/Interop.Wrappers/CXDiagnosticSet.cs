using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXDiagnosticSet : IEquatable<CXDiagnosticSet>
    {
        public CXDiagnosticSet(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXDiagnosticSet(void* value) => new CXDiagnosticSet((IntPtr)value);

        public static implicit operator void*(CXDiagnosticSet value) => (void*)value.Handle;

        public static bool operator ==(CXDiagnosticSet left, CXDiagnosticSet right) => left.Handle == right.Handle;

        public static bool operator !=(CXDiagnosticSet left, CXDiagnosticSet right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXDiagnosticSet other) && Equals(other);

        public bool Equals(CXDiagnosticSet other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
