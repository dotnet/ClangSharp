using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXEvalResult : IDisposable, IEquatable<CXEvalResult>
    {
        public CXEvalResult(IntPtr handle)
        {
            Handle = handle;
        }

        public double AsDouble => clang.EvalResult_getAsDouble(this);

        public int AsInt => clang.EvalResult_getAsInt(this);

        public long AsLongLong => clang.EvalResult_getAsLongLong(this);

        public string AsStr
        {
            get
            {
                var pStr = clang.EvalResult_getAsStr(this);

                if (pStr is null)
                {
                    return string.Empty;
                }

                var span = new ReadOnlySpan<byte>(pStr, int.MaxValue);
                return span.Slice(0, span.IndexOf((byte)'\0')).AsString();
            }
        }

        public ulong AsUnsigned => clang.EvalResult_getAsUnsigned(this);

        public IntPtr Handle { get; set; }

        public bool IsUnsignedInt => clang.EvalResult_isUnsignedInt(this) != 0;

        public CXEvalResultKind Kind => clang.EvalResult_getKind(this);

        public static explicit operator CXEvalResult(void* value) => new CXEvalResult((IntPtr)value);

        public static implicit operator void*(CXEvalResult value) => (void*)value.Handle;

        public static bool operator ==(CXEvalResult left, CXEvalResult right) => left.Handle == right.Handle;

        public static bool operator !=(CXEvalResult left, CXEvalResult right) => left.Handle != right.Handle;

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.EvalResult_dispose(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXEvalResult other) && Equals(other);

        public bool Equals(CXEvalResult other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
