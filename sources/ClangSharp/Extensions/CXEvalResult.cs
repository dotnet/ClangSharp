using System;

namespace ClangSharp
{
    public unsafe partial struct CXEvalResult : IDisposable
    {
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

        public bool IsUnsignedInt => clang.EvalResult_isUnsignedInt(this) != 0;

        public CXEvalResultKind Kind => clang.EvalResult_getKind(this);

        public void Dispose()
        {
            if (Pointer != IntPtr.Zero)
            {
                clang.EvalResult_dispose(this);
                Pointer = IntPtr.Zero;
            }
        }
    }
}
