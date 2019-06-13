using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompilationDatabase : IEquatable<CXCompilationDatabase>
    {
        public CXCompilationDatabase(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXCompilationDatabase(void* value) => new CXCompilationDatabase((IntPtr)value);

        public static implicit operator void*(CXCompilationDatabase value) => (void*)value.Handle;

        public static bool operator ==(CXCompilationDatabase left, CXCompilationDatabase right) => left.Handle == right.Handle;

        public static bool operator !=(CXCompilationDatabase left, CXCompilationDatabase right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXCompilationDatabase other) && Equals(other);

        public bool Equals(CXCompilationDatabase other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
