using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIndex : IDisposable, IEquatable<CXIndex>
    {
        public CXIndex(IntPtr handle)
        {
            Handle = handle;
        }

        public CXGlobalOptFlags GlobalOptions
        {
            get => (CXGlobalOptFlags)clang.CXIndex_getGlobalOptions(this);
            set => clang.CXIndex_setGlobalOptions(this, (uint)value);
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXIndex(void* value) => new CXIndex((IntPtr)value);

        public static implicit operator void*(CXIndex value) => (void*)value.Handle;

        public static bool operator ==(CXIndex left, CXIndex right) => left.Handle == right.Handle;

        public static bool operator !=(CXIndex left, CXIndex right) => left.Handle != right.Handle;

        public static CXIndex Create(bool excludeDeclarationsFromPch = false, bool displayDiagnostics = false) => (CXIndex)clang.createIndex(excludeDeclarationsFromPch ? 1 : 0, displayDiagnostics ? 1 : 0);

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.disposeIndex(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXIndex other) && Equals(other);

        public bool Equals(CXIndex other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
