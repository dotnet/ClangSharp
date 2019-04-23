using System;

namespace ClangSharp
{
    public partial struct CXString : IDisposable
    {
        public string CString => clang.getCString(this);

        public void Dispose() => clang.disposeString(this);

        public override string ToString() => CString;
    }
}
