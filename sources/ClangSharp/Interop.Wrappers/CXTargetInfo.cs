using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXTargetInfo : IEquatable<CXTargetInfo>
    {
        public CXTargetInfo(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static implicit operator CXTargetInfo(CXTargetInfoImpl* value) => new CXTargetInfo((IntPtr)value);

        public static implicit operator CXTargetInfoImpl*(CXTargetInfo value) => (CXTargetInfoImpl*)value.Handle;

        public static bool operator ==(CXTargetInfo left, CXTargetInfo right) => left.Handle == right.Handle;

        public static bool operator !=(CXTargetInfo left, CXTargetInfo right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXTargetInfo other) && Equals(other);

        public bool Equals(CXTargetInfo other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
