using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXString : IDisposable
    {
        public string CString
        {
            get
            {
                var pCString = clang.getCString(this);

                if (pCString is null)
                {
                    return string.Empty;
                }

                var span = new ReadOnlySpan<byte>(pCString, int.MaxValue);
                return span.Slice(0, span.IndexOf((byte)'\0')).AsString();
            }
        }

        public void Dispose() => clang.disposeString(this);

        public override string ToString() => CString;
    }
}
