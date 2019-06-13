using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXVirtualFileOverlay : IEquatable<CXVirtualFileOverlay>
    {
        public CXVirtualFileOverlay(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static implicit operator CXVirtualFileOverlay(CXVirtualFileOverlayImpl* value) => new CXVirtualFileOverlay((IntPtr)value);

        public static implicit operator CXVirtualFileOverlayImpl*(CXVirtualFileOverlay value) => (CXVirtualFileOverlayImpl*)value.Handle;

        public static bool operator ==(CXVirtualFileOverlay left, CXVirtualFileOverlay right) => left.Handle == right.Handle;

        public static bool operator !=(CXVirtualFileOverlay left, CXVirtualFileOverlay right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXVirtualFileOverlay other) && Equals(other);

        public bool Equals(CXVirtualFileOverlay other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
