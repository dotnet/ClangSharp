using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXRemapping : IEquatable<CXRemapping>
    {
        public CXRemapping(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXRemapping(void* value) => new CXRemapping((IntPtr)value);

        public static implicit operator void*(CXRemapping value) => (void*)value.Handle;

        public static bool operator ==(CXRemapping left, CXRemapping right) => left.Handle == right.Handle;

        public static bool operator !=(CXRemapping left, CXRemapping right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXRemapping other) && Equals(other);

        public bool Equals(CXRemapping other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
