using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXRemapping : IDisposable, IEquatable<CXRemapping>
    {
        public CXRemapping(IntPtr handle)
        {
            Handle = handle;
        }

        public static CXRemapping GetRemappings(string path)
        {
            using var marshaledPath = new MarshaledString(path);
            return (CXRemapping)clang.getRemappings(marshaledPath);
        }

        public static CXRemapping GetRemappingsFromFileList(ReadOnlySpan<string> filePaths)
        {
            using var marshaledPaths = new MarshaledStringArray(filePaths);
            var pMarshaledPaths = stackalloc sbyte*[filePaths.Length];

            marshaledPaths.Fill(pMarshaledPaths);
            return (CXRemapping)clang.getRemappingsFromFileList(pMarshaledPaths, (uint)filePaths.Length);
        }

        public IntPtr Handle { get; set; }

        public uint NumFiles => clang.remap_getNumFiles(this);

        public static explicit operator CXRemapping(void* value) => new CXRemapping((IntPtr)value);

        public static implicit operator void*(CXRemapping value) => (void*)value.Handle;

        public static bool operator ==(CXRemapping left, CXRemapping right) => left.Handle == right.Handle;

        public static bool operator !=(CXRemapping left, CXRemapping right) => left.Handle != right.Handle;

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.remap_dispose(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXRemapping other) && Equals(other);

        public bool Equals(CXRemapping other) => this == other;

        public void GetFilenames(uint index, out CXString original, out CXString transformed)
        {
            fixed (CXString* pOriginal = &original)
            fixed (CXString* pTransformed = &transformed)
            {
                clang.remap_getFilenames(this, index, pOriginal, pTransformed);
            }
        }

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
