using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXTargetInfo : IDisposable, IEquatable<CXTargetInfo>
    {
        public CXTargetInfo(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public int PointerWidth => clang.TargetInfo_getPointerWidth(this);

        public CXString Triple => clang.TargetInfo_getTriple(this);

        public static implicit operator CXTargetInfo(CXTargetInfoImpl* value) => new CXTargetInfo((IntPtr)value);

        public static implicit operator CXTargetInfoImpl*(CXTargetInfo value) => (CXTargetInfoImpl*)value.Handle;

        public static bool operator ==(CXTargetInfo left, CXTargetInfo right) => left.Handle == right.Handle;

        public static bool operator !=(CXTargetInfo left, CXTargetInfo right) => left.Handle != right.Handle;

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.TargetInfo_dispose(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXTargetInfo other) && Equals(other);

        public bool Equals(CXTargetInfo other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        public override string ToString() => Triple.ToString();
    }
}
