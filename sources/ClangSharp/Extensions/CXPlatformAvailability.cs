using System;

namespace ClangSharp
{
    public partial struct CXPlatformAvailability : IDisposable
    {
        public void Dispose() => clang.disposeCXPlatformAvailability(ref this);
    }
}
