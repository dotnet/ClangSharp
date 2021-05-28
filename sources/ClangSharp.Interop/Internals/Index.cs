// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NETSTANDARD2_0
using System.Runtime.CompilerServices;

namespace System
{
    internal readonly struct Index : IEquatable<Index>
    {
        private readonly int _value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Index(int value, bool fromEnd = false)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            _value = fromEnd ? ~value : value;
        }

        private Index(int value)
        {
            _value = value;
        }

        public static Index Start => new Index(0);

        public static Index End => new Index(~0);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromStart(int value) => value < 0 ? throw new ArgumentOutOfRangeException(nameof(value)) : new Index(value);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Index FromEnd(int value) => value < 0 ? throw new ArgumentOutOfRangeException(nameof(value)) : new Index(~value);

        public int Value => _value < 0 ? ~_value : _value;

        public bool IsFromEnd => _value < 0;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int GetOffset(int length)
        {
            var offset = _value;
            if (IsFromEnd)
            {
                offset += length + 1;
            }
            return offset;
        }

        public override bool Equals(object value) => value is Index index && _value == index._value;

        public bool Equals(Index other) => _value == other._value;

        public override int GetHashCode() => _value;

        public static implicit operator Index(int value) => FromStart(value);

        public override string ToString() => IsFromEnd ? ToStringFromEnd() : ((uint)Value).ToString();

        private string ToStringFromEnd() => '^' + Value.ToString();
    }
}
#endif
