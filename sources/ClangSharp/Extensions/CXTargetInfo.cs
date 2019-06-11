using System;

namespace ClangSharp
{
    public unsafe partial struct CXTargetInfo : IDisposable
    {
        public int PointerWidth => clang.TargetInfo_getPointerWidth(this);

        public CXString Triple => clang.TargetInfo_getTriple(this);

        public void Dispose()
        {
            if (Pointer != IntPtr.Zero)
            {
                clang.TargetInfo_dispose(this);
                Pointer = IntPtr.Zero;
            }
        }
    }
}
