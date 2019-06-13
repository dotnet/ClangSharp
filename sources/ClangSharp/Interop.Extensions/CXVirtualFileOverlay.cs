using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXVirtualFileOverlay : IDisposable
    {
        public static CXVirtualFileOverlay Create(uint options)
        {
            return clang.VirtualFileOverlay_create(options);
        }

        public void Dispose()
        {
            if (Pointer != IntPtr.Zero)
            {
                clang.VirtualFileOverlay_dispose(this);
                Pointer = IntPtr.Zero;
            }
        }

        public CXErrorCode AddFileMapping(string virtualPath, string realPath)
        {
            using (var marshaledVirtualPath = new MarshaledString(virtualPath))
            using (var marshaledRealPath = new MarshaledString(realPath))
            {
                return clang.VirtualFileOverlay_addFileMapping(this, marshaledVirtualPath, marshaledRealPath);
            }
        }

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
