using System;
using System.Diagnostics.CodeAnalysis;

namespace ClangSharp.Test
{
    public partial struct Outer : IEquatable<Outer>
    {
        public int Id;

        [NativeTypeName("__AnonymousRecord_ClangUnsavedFile_L4_C5")]
        public _Anonymous_e__Struct Anonymous;

        [UnscopedRef]
        public ref int First
        {
            get
            {
                return ref Anonymous.First;
            }
        }

        [UnscopedRef]
        public ref int Second
        {
            get
            {
                return ref Anonymous.Second;
            }
        }

        public partial struct _Anonymous_e__Struct : IEquatable<_Anonymous_e__Struct>
        {
            public int First;

            public int Second;

            public override readonly bool Equals(object? obj) => (obj is _Anonymous_e__Struct other) && Equals(other);

            public readonly bool Equals(_Anonymous_e__Struct other) => (First == other.First) && (Second == other.Second);

            public override readonly int GetHashCode() => HashCode.Combine(First, Second);

            public static bool operator ==(_Anonymous_e__Struct left, _Anonymous_e__Struct right) => left.Equals(right);

            public static bool operator !=(_Anonymous_e__Struct left, _Anonymous_e__Struct right) => !(left == right);
        }

        public override readonly bool Equals(object? obj) => (obj is Outer other) && Equals(other);

        public readonly bool Equals(Outer other) => (Id == other.Id) && Anonymous.Equals(other.Anonymous);

        public override readonly int GetHashCode() => HashCode.Combine(Id, Anonymous);

        public static bool operator ==(Outer left, Outer right) => left.Equals(right);

        public static bool operator !=(Outer left, Outer right) => !(left == right);
    }
}
