using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompileCommands : IEquatable<CXCompileCommands>
    {
        public CXCompileCommands(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXCompileCommands(void* value) => new CXCompileCommands((IntPtr)value);

        public static implicit operator void*(CXCompileCommands value) => (void*)value.Handle;

        public static bool operator ==(CXCompileCommands left, CXCompileCommands right) => left.Handle == right.Handle;

        public static bool operator !=(CXCompileCommands left, CXCompileCommands right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXCompileCommands other) && Equals(other);

        public bool Equals(CXCompileCommands other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
