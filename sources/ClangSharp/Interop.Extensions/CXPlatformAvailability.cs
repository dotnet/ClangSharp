using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXPlatformAvailability : IDisposable
    {
        public void Dispose()
        {
            fixed (CXPlatformAvailability* pThis = &this)
            {
                clang.disposeCXPlatformAvailability(pThis);
            }
        }
    }
}
