using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXVirtualFileOverlay : IDisposable, IEquatable<CXVirtualFileOverlay>
    {
        public CXVirtualFileOverlay(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static implicit operator CXVirtualFileOverlay(CXVirtualFileOverlayImpl* value) => new CXVirtualFileOverlay((IntPtr)value);

        public static implicit operator CXVirtualFileOverlayImpl*(CXVirtualFileOverlay value) => (CXVirtualFileOverlayImpl*)value.Handle;

        public static bool operator ==(CXVirtualFileOverlay left, CXVirtualFileOverlay right) => left.Handle == right.Handle;

        public static bool operator !=(CXVirtualFileOverlay left, CXVirtualFileOverlay right) => left.Handle != right.Handle;

        public static CXVirtualFileOverlay Create(uint options)
        {
            return clang.VirtualFileOverlay_create(options);
        }

        public CXErrorCode AddFileMapping(string virtualPath, string realPath)
        {
            using (var marshaledVirtualPath = new MarshaledString(virtualPath))
            using (var marshaledRealPath = new MarshaledString(realPath))
            {
                return clang.VirtualFileOverlay_addFileMapping(this, marshaledVirtualPath, marshaledRealPath);
            }
        }

        public void Dispose()
        {
            if (Handle != IntPtr.Zero)
            {
                clang.VirtualFileOverlay_dispose(this);
                Handle = IntPtr.Zero;
            }
        }

        public override bool Equals(object obj) => (obj is CXVirtualFileOverlay other) && Equals(other);

        public bool Equals(CXVirtualFileOverlay other) => (this == other);

        public override int GetHashCode() => Handle.GetHashCode();

        public CXErrorCode SetCaseSensitivity(bool caseSensitive)
        {
            return clang.VirtualFileOverlay_setCaseSensitivity(this, caseSensitive ? 1 : 0);
        }

        public Span<byte> WriteToBuffer(uint options, out CXErrorCode errorCode)
        {
            sbyte* pBuffer; uint size;
            errorCode = clang.VirtualFileOverlay_writeToBuffer(this, options, &pBuffer, &size);
            return new Span<byte>(pBuffer, (int)size);
        }
    }
}
