using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompileCommand : IEquatable<CXCompileCommand>
    {
        public CXCompileCommand(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXCompileCommand(void* value) => new CXCompileCommand((IntPtr)value);

        public static implicit operator void*(CXCompileCommand value) => (void*)value.Handle;

        public static bool operator ==(CXCompileCommand left, CXCompileCommand right) => left.Handle == right.Handle;

        public static bool operator !=(CXCompileCommand left, CXCompileCommand right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXCompileCommand other) && Equals(other);

        public bool Equals(CXCompileCommand other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
