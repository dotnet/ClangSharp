using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIndexAction : IEquatable<CXIndexAction>
    {
        public CXIndexAction(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXIndexAction(void* value) => new CXIndexAction((IntPtr)value);

        public static implicit operator void*(CXIndexAction value) => (void*)value.Handle;

        public static bool operator ==(CXIndexAction left, CXIndexAction right) => left.Handle == right.Handle;

        public static bool operator !=(CXIndexAction left, CXIndexAction right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXIndexAction other) && Equals(other);

        public bool Equals(CXIndexAction other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
