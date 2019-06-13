using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXTargetInfo : IDisposable
    {
        public int PointerWidth => clang.TargetInfo_getPointerWidth(this);

        public CXString Triple => clang.TargetInfo_getTriple(this);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.TargetInfo_dispose(this);
                Handle = IntPtr.Zero;
            }
        }
    }
}
