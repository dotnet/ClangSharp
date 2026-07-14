using System;

namespace ClangSharp.Test
{
    public partial struct Base : IEquatable<Base>
    {
        public int X;

        public int Y;

        public override readonly bool Equals(object? obj) => (obj is Base other) && Equals(other);

        public readonly bool Equals(Base other) => (X == other.X) && (Y == other.Y);

        public override readonly int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Base left, Base right) => left.Equals(right);

        public static bool operator !=(Base left, Base right) => !(left == right);
    }

    [NativeTypeName("struct Derived : Base")]
    public partial struct Derived : IEquatable<Derived>
    {
        public Base Base;

        public int Z;

        public override readonly bool Equals(object? obj) => (obj is Derived other) && Equals(other);

        public readonly bool Equals(Derived other) => Base.Equals(other.Base) && (Z == other.Z);

        public override readonly int GetHashCode() => HashCode.Combine(Base, Z);

        public static bool operator ==(Derived left, Derived right) => left.Equals(right);

        public static bool operator !=(Derived left, Derived right) => !(left == right);
    }
}
