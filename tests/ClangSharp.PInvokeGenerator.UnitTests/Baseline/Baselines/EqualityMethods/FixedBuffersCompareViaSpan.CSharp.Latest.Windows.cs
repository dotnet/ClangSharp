using System;
using System.Runtime.CompilerServices;

namespace ClangSharp.Test
{
    public partial struct WithArray : IEquatable<WithArray>
    {
        [NativeTypeName("int[4]")]
        public _Values_e__FixedBuffer Values;

        public float Scale;

        [InlineArray(4)]
        public partial struct _Values_e__FixedBuffer
        {
            public int e0;
        }

        public override readonly bool Equals(object? obj) => (obj is WithArray other) && Equals(other);

        public readonly bool Equals(WithArray other) => ((ReadOnlySpan<int>)Values).SequenceEqual(other.Values) && (Scale == other.Scale);

        public override readonly int GetHashCode()
        {
            var hashCode = new HashCode();
            foreach (var element in (ReadOnlySpan<int>)Values)
            {
                hashCode.Add(element);
            }
            hashCode.Add(Scale);
            return hashCode.ToHashCode();
        }

        public static bool operator ==(WithArray left, WithArray right) => left.Equals(right);

        public static bool operator !=(WithArray left, WithArray right) => !(left == right);
    }
}
