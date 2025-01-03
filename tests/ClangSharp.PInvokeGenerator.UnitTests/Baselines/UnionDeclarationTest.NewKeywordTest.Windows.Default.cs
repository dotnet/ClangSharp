using System.Runtime.InteropServices;

namespace ClangSharp.Test
{
    [StructLayout(LayoutKind.Explicit)]
    public partial struct MyUnion
    {
        [FieldOffset(0)]
        public new int Equals;

        [FieldOffset(0)]
        public int Dispose;

        [FieldOffset(0)]
        public new int GetHashCode;

        [FieldOffset(0)]
        public new int GetType;

        [FieldOffset(0)]
        public new int MemberwiseClone;

        [FieldOffset(0)]
        public new int ReferenceEquals;

        [FieldOffset(0)]
        public new int ToString;
    }
}
