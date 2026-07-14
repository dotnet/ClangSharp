using System;

namespace ClangSharp.Test
{
    public partial struct Flags : IEquatable<Flags>
    {
        public uint _bitfield;

        [NativeTypeName("unsigned int : 1")]
        public uint A
        {
            readonly get
            {
                return _bitfield & 0x1u;
            }

            set
            {
                _bitfield = (_bitfield & ~0x1u) | (value & 0x1u);
            }
        }

        [NativeTypeName("unsigned int : 2")]
        public uint B
        {
            readonly get
            {
                return (_bitfield >> 1) & 0x3u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x3u << 1)) | ((value & 0x3u) << 1);
            }
        }

        [NativeTypeName("unsigned int : 3")]
        public uint C
        {
            readonly get
            {
                return (_bitfield >> 3) & 0x7u;
            }

            set
            {
                _bitfield = (_bitfield & ~(0x7u << 3)) | ((value & 0x7u) << 3);
            }
        }

        public int Value;

        public override readonly bool Equals(object? obj) => (obj is Flags other) && Equals(other);

        public readonly bool Equals(Flags other) => (_bitfield == other._bitfield) && (Value == other.Value);

        public override readonly int GetHashCode() => HashCode.Combine(_bitfield, Value);

        public static bool operator ==(Flags left, Flags right) => left.Equals(right);

        public static bool operator !=(Flags left, Flags right) => !(left == right);
    }
}
