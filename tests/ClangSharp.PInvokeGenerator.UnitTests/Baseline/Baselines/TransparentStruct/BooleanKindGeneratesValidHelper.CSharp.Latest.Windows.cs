using System;
using System.Diagnostics;

namespace ClangSharp.Test
{
    /// <summary>Defines the type of a member as it was used in the native signature.</summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
    [Conditional("DEBUG")]
    internal sealed partial class NativeTypeNameAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref="NativeTypeNameAttribute" /> class.</summary>
        /// <param name="name">The name of the type that was used in the native signature.</param>
        public NativeTypeNameAttribute(string name)
        {
            _name = name;
        }

        /// <summary>Gets the name of the type that was used in the native signature.</summary>
        public string Name => _name;
    }

    /// <summary>Defines the annotation found in a native declaration.</summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
    [Conditional("DEBUG")]
    internal sealed partial class NativeAnnotationAttribute : Attribute
    {
        private readonly string _annotation;

        /// <summary>Initializes a new instance of the <see cref="NativeAnnotationAttribute" /> class.</summary>
        /// <param name="annotation">The annotation that was used in the native declaration.</param>
        public NativeAnnotationAttribute(string annotation)
        {
            _annotation = annotation;
        }

        /// <summary>Gets the annotation that was used in the native declaration.</summary>
        public string Annotation => _annotation;
    }

    public readonly partial struct MyBoolean : IComparable, IComparable<MyBoolean>, IEquatable<MyBoolean>, IFormattable
    {
        public readonly byte Value;

        public MyBoolean(byte value)
        {
            Value = value;
        }

        public static MyBoolean FALSE => new MyBoolean(0);

        public static MyBoolean TRUE => new MyBoolean(1);

        public static bool operator ==(MyBoolean left, MyBoolean right) => left.Value == right.Value;

        public static bool operator !=(MyBoolean left, MyBoolean right) => left.Value != right.Value;

        public static bool operator <(MyBoolean left, MyBoolean right) => left.Value < right.Value;

        public static bool operator <=(MyBoolean left, MyBoolean right) => left.Value <= right.Value;

        public static bool operator >(MyBoolean left, MyBoolean right) => left.Value > right.Value;

        public static bool operator >=(MyBoolean left, MyBoolean right) => left.Value >= right.Value;

        public static implicit operator bool(MyBoolean value) => value.Value != 0;

        public static implicit operator MyBoolean(bool value) => new MyBoolean((byte)(value ? 1u : 0u));

        public static bool operator false(MyBoolean value) => value.Value == 0;

        public static bool operator true(MyBoolean value) => value.Value != 0;

        public static implicit operator MyBoolean(byte value) => new MyBoolean(value);

        public static implicit operator byte(MyBoolean value) => value.Value;

        public static explicit operator MyBoolean(short value) => new MyBoolean(unchecked((byte)(value)));

        public static implicit operator short(MyBoolean value) => value.Value;

        public static explicit operator MyBoolean(int value) => new MyBoolean(unchecked((byte)(value)));

        public static implicit operator int(MyBoolean value) => value.Value;

        public static explicit operator MyBoolean(long value) => new MyBoolean(unchecked((byte)(value)));

        public static implicit operator long(MyBoolean value) => value.Value;

        public static explicit operator MyBoolean(nint value) => new MyBoolean(unchecked((byte)(value)));

        public static implicit operator nint(MyBoolean value) => value.Value;

        public static explicit operator MyBoolean(sbyte value) => new MyBoolean(unchecked((byte)(value)));

        public static explicit operator sbyte(MyBoolean value) => (sbyte)(value.Value);

        public static explicit operator MyBoolean(ushort value) => new MyBoolean(unchecked((byte)(value)));

        public static implicit operator ushort(MyBoolean value) => value.Value;

        public static explicit operator MyBoolean(uint value) => new MyBoolean(unchecked((byte)(value)));

        public static implicit operator uint(MyBoolean value) => value.Value;

        public static explicit operator MyBoolean(ulong value) => new MyBoolean(unchecked((byte)(value)));

        public static implicit operator ulong(MyBoolean value) => value.Value;

        public static explicit operator MyBoolean(nuint value) => new MyBoolean(unchecked((byte)(value)));

        public static implicit operator nuint(MyBoolean value) => value.Value;

        public int CompareTo(object? obj)
        {
            if (obj is MyBoolean other)
            {
                return CompareTo(other);
            }

            return (obj is null) ? 1 : throw new ArgumentException("obj is not an instance of MyBoolean.");
        }

        public int CompareTo(MyBoolean other) => Value.CompareTo(other.Value);

        public override bool Equals(object? obj) => (obj is MyBoolean other) && Equals(other);

        public bool Equals(MyBoolean other) => Value.Equals(other.Value);

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value.ToString();

        public string ToString(string? format, IFormatProvider? formatProvider) => Value.ToString(format, formatProvider);
    }
}
