using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCursorSet : IEquatable<CXCursorSet>
    {
        public CXCursorSet(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static implicit operator CXCursorSet(CXCursorSetImpl* value) => new CXCursorSet((IntPtr)value);

        public static implicit operator CXCursorSetImpl*(CXCursorSet value) => (CXCursorSetImpl*)value.Handle;

        public static bool operator ==(CXCursorSet left, CXCursorSet right) => left.Handle == right.Handle;

        public static bool operator !=(CXCursorSet left, CXCursorSet right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXCursorSet other) && Equals(other);

        public bool Equals(CXCursorSet other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
