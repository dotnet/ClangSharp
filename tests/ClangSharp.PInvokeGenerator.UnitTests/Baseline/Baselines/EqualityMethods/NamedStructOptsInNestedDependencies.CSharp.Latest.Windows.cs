using System;

namespace ClangSharp.Test
{
    public partial struct Point : IEquatable<Point>
    {
        public int X;

        public int Y;

        public override readonly bool Equals(object? obj) => (obj is Point other) && Equals(other);

        public readonly bool Equals(Point other) => (X == other.X) && (Y == other.Y);

        public override readonly int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Point left, Point right) => left.Equals(right);

        public static bool operator !=(Point left, Point right) => !(left == right);
    }

    public partial struct MyData : IEquatable<MyData>
    {
        public int Value;

        public Point Origin;

        public override readonly bool Equals(object? obj) => (obj is MyData other) && Equals(other);

        public readonly bool Equals(MyData other) => (Value == other.Value) && Origin.Equals(other.Origin);

        public override readonly int GetHashCode() => HashCode.Combine(Value, Origin);

        public static bool operator ==(MyData left, MyData right) => left.Equals(right);

        public static bool operator !=(MyData left, MyData right) => !(left == right);
    }

    public partial struct Other
    {
        public int Unrelated;
    }
}
