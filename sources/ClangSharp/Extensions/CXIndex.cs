using System;

namespace ClangSharp
{
    public unsafe partial struct CXIndex : IDisposable
    {
        public static CXIndex Create(bool excludeDeclarationsFromPch = false, bool displayDiagnostics = false) => (CXIndex)clang.createIndex(excludeDeclarationsFromPch ? 1 : 0, displayDiagnostics ? 1 : 0);

        public CXGlobalOptFlags GlobalOptions
        {
            get => (CXGlobalOptFlags)clang.CXIndex_getGlobalOptions(this);
            set => clang.CXIndex_setGlobalOptions(this, (uint)value);
        }

        public void Dispose()
        {
            if (Pointer != IntPtr.Zero)
            {
                clang.disposeIndex(this);
                Pointer = IntPtr.Zero;
            }
        }
    }
}
