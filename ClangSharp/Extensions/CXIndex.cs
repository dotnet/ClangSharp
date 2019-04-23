using System;

namespace ClangSharp
{
    public partial struct CXIndex : IDisposable
    {
        public CXIndex Create(bool excludeDeclarationsFromPch = false, bool displayDiagnostics = false)
            => clang.createIndex(excludeDeclarationsFromPch ? 1 : 0, displayDiagnostics ? 1 : 0);

        public CXGlobalOptFlags GlobalOptions
        {
            get => (CXGlobalOptFlags)clang.CXIndex_getGlobalOptions(this);
            set => clang.CXIndex_setGlobalOptions(this, (uint)value);
        }

        public void Dispose()
            => clang.disposeIndex(this);
    }
}
