using System;

namespace ClangSharp
{
    public partial struct CXTargetInfo : IDisposable
    {
        public int PointerWidth => clang.TargetInfo_getPointerWidth(this);

        public CXString Triple => clang.TargetInfo_getTriple(this);

        public void Dispose() => clang.TargetInfo_dispose(this);
    }
}
