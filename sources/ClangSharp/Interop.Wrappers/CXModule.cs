using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXModule : IEquatable<CXModule>
    {
        public CXModule(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXModule(void* value) => new CXModule((IntPtr)value);

        public static implicit operator void*(CXModule value) => (void*)value.Handle;

        public static bool operator ==(CXModule left, CXModule right) => left.Handle == right.Handle;

        public static bool operator !=(CXModule left, CXModule right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXModule other) && Equals(other);

        public bool Equals(CXModule other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
