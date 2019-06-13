using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXFile : IEquatable<CXFile>
    {
        public CXFile(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXFile(void* value) => new CXFile((IntPtr)value);

        public static implicit operator void*(CXFile value) => (void*)value.Handle;

        public static bool operator ==(CXFile left, CXFile right) => clang.File_isEqual(left, right) != 0;

        public static bool operator !=(CXFile left, CXFile right) => clang.File_isEqual(left, right) == 0;

        public override bool Equals(object obj) => (obj is CXFile other) && Equals(other);

        public bool Equals(CXFile other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
