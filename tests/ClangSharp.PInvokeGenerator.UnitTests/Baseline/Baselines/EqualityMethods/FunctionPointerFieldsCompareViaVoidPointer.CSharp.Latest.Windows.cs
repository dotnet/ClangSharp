using System;

namespace ClangSharp.Test
{
    public unsafe partial struct WithCallback : IEquatable<WithCallback>
    {
        public int Id;

        [NativeTypeName("void (*)(int)")]
        public delegate* unmanaged[Cdecl]<int, void> Callback;

        public override readonly bool Equals(object? obj) => (obj is WithCallback other) && Equals(other);

        public readonly bool Equals(WithCallback other) => (Id == other.Id) && ((void*)Callback == (void*)other.Callback);

        public override readonly int GetHashCode() => HashCode.Combine(Id, (nint)(void*)Callback);

        public static bool operator ==(WithCallback left, WithCallback right) => left.Equals(right);

        public static bool operator !=(WithCallback left, WithCallback right) => !(left == right);
    }
}
