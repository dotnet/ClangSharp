using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXPrintingPolicy : IEquatable<CXPrintingPolicy>
    {
        public CXPrintingPolicy(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXPrintingPolicy(void* value) => new CXPrintingPolicy((IntPtr)value);

        public static implicit operator void*(CXPrintingPolicy value) => (void*)value.Handle;

        public static bool operator ==(CXPrintingPolicy left, CXPrintingPolicy right) => left.Handle == right.Handle;

        public static bool operator !=(CXPrintingPolicy left, CXPrintingPolicy right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXPrintingPolicy other) && Equals(other);

        public bool Equals(CXPrintingPolicy other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
