using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompletionString : IEquatable<CXCompletionString>
    {
        public CXCompletionString(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXCompletionString(void* value) => new CXCompletionString((IntPtr)value);

        public static implicit operator void*(CXCompletionString value) => (void*)value.Handle;

        public static bool operator ==(CXCompletionString left, CXCompletionString right) => left.Handle == right.Handle;

        public static bool operator !=(CXCompletionString left, CXCompletionString right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXCompletionString other) && Equals(other);

        public bool Equals(CXCompletionString other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
