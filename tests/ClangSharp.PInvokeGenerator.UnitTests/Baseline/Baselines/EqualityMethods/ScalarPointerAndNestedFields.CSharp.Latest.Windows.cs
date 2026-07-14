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

    public unsafe partial struct MyData : IEquatable<MyData>
    {
        public int Value;

        public float Ratio;

        public Point Origin;

        public void* Handle;

        public override readonly bool Equals(object? obj) => (obj is MyData other) && Equals(other);

        public readonly bool Equals(MyData other) => (Value == other.Value) && (Ratio == other.Ratio) && Origin.Equals(other.Origin) && (Handle == other.Handle);

        public override readonly int GetHashCode() => HashCode.Combine(Value, Ratio, Origin, (nint)Handle);

        public static bool operator ==(MyData left, MyData right) => left.Equals(right);

        public static bool operator !=(MyData left, MyData right) => !(left == right);
    }
}
