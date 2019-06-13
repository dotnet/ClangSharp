using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXDiagnosticSet : IDisposable, IEquatable<CXDiagnosticSet>, IReadOnlyCollection<CXDiagnostic>
    {
        public CXDiagnosticSet(IntPtr handle)
        {
            Handle = handle;
        }

        public CXDiagnostic this[uint index] => GetDiagnostic(index);

        public int Count => (int)clang.getNumDiagnosticsInSet(this);

        public IntPtr Handle { get; set; }

        public static explicit operator CXDiagnosticSet(void* value) => new CXDiagnosticSet((IntPtr)value);

        public static implicit operator void*(CXDiagnosticSet value) => (void*)value.Handle;

        public static bool operator ==(CXDiagnosticSet left, CXDiagnosticSet right) => left.Handle == right.Handle;

        public static bool operator !=(CXDiagnosticSet left, CXDiagnosticSet right) => left.Handle != right.Handle;

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.disposeDiagnosticSet(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXDiagnosticSet other) && Equals(other);

        public bool Equals(CXDiagnosticSet other) => (this == other);

        public CXDiagnostic GetDiagnostic(uint index) => (CXDiagnostic)clang.getDiagnosticInSet(this, index);

        public IEnumerator<CXDiagnostic> GetEnumerator()
        {
            var count = (uint)Count;

            for (var index = 0u; index < count; index++)
            {
                yield return GetDiagnostic(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
