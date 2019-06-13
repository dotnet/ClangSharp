using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXEvalResult : IEquatable<CXEvalResult>
    {
        public CXEvalResult(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXEvalResult(void* value) => new CXEvalResult((IntPtr)value);

        public static implicit operator void*(CXEvalResult value) => (void*)value.Handle;

        public static bool operator ==(CXEvalResult left, CXEvalResult right) => left.Handle == right.Handle;

        public static bool operator !=(CXEvalResult left, CXEvalResult right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXEvalResult other) && Equals(other);

        public bool Equals(CXEvalResult other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
