using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIndex : IEquatable<CXIndex>
    {
        public CXIndex(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXIndex(void* value) => new CXIndex((IntPtr)value);

        public static implicit operator void*(CXIndex value) => (void*)value.Handle;

        public static bool operator ==(CXIndex left, CXIndex right) => left.Handle == right.Handle;

        public static bool operator !=(CXIndex left, CXIndex right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXIndex other) && Equals(other);

        public bool Equals(CXIndex other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
