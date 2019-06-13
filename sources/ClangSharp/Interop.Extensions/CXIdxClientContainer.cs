using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxClientContainer : IEquatable<CXIdxClientContainer>
    {
        public CXIdxClientContainer(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXIdxClientContainer(void* value) => new CXIdxClientContainer((IntPtr)value);

        public static implicit operator void*(CXIdxClientContainer value) => (void*)value.Handle;

        public static bool operator ==(CXIdxClientContainer left, CXIdxClientContainer right) => left.Handle == right.Handle;

        public static bool operator !=(CXIdxClientContainer left, CXIdxClientContainer right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXIdxClientContainer other) && Equals(other);

        public bool Equals(CXIdxClientContainer other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
