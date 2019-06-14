using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompilationDatabase : IDisposable, IEquatable<CXCompilationDatabase>
    {
        public CXCompilationDatabase(IntPtr handle)
        {
            Handle = handle;
        }

        public CXCompileCommands AllCompileCommands => (CXCompileCommands)clang.CompilationDatabase_getAllCompileCommands(this);

        public IntPtr Handle { get; set; }

        public static explicit operator CXCompilationDatabase(void* value) => new CXCompilationDatabase((IntPtr)value);

        public static implicit operator void*(CXCompilationDatabase value) => (void*)value.Handle;

        public static bool operator ==(CXCompilationDatabase left, CXCompilationDatabase right) => left.Handle == right.Handle;

        public static bool operator !=(CXCompilationDatabase left, CXCompilationDatabase right) => left.Handle != right.Handle;

        public static CXCompilationDatabase FromDirectory(string buildDir, out CXCompilationDatabase_Error errorCode)
        {
            using var marshaledBuildDir = new MarshaledString(buildDir);

            fixed (CXCompilationDatabase_Error* pErrorCode = &errorCode)
            {
                return (CXCompilationDatabase)clang.CompilationDatabase_fromDirectory(marshaledBuildDir, pErrorCode);
            }
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.CompilationDatabase_dispose(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXCompilationDatabase other) && Equals(other);

        public bool Equals(CXCompilationDatabase other) => this == other;

        public CXCompileCommands GetCompileCommands(string completeFileName)
        {
            using var marshaledCompleteFileName = new MarshaledString(completeFileName);
            return (CXCompileCommands)clang.CompilationDatabase_getCompileCommands(this, marshaledCompleteFileName);
        }

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
