// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

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

    public static CXVirtualFileOverlay Create(uint options) => clang.VirtualFileOverlay_create(options);

    public readonly CXErrorCode AddFileMapping(string? virtualPath, string? realPath)
    {
        using var marshaledVirtualPath = new MarshaledString(virtualPath);
        using var marshaledRealPath = new MarshaledString(realPath);
        return clang.VirtualFileOverlay_addFileMapping(this, marshaledVirtualPath, marshaledRealPath);
    }

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            clang.VirtualFileOverlay_dispose(this);
            Handle = IntPtr.Zero;
        }
    }

    public override readonly bool Equals(object? obj) => (obj is CXVirtualFileOverlay other) && Equals(other);

    public readonly bool Equals(CXVirtualFileOverlay other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly CXErrorCode SetCaseSensitivity(bool caseSensitive) => clang.VirtualFileOverlay_setCaseSensitivity(this, caseSensitive ? 1 : 0);

    public readonly Span<byte> WriteToBuffer(uint options, out CXErrorCode errorCode)
    {
        sbyte* pBuffer; uint size;
        errorCode = clang.VirtualFileOverlay_writeToBuffer(this, options, &pBuffer, &size);

#if NETSTANDARD
        var result = new byte[checked((int)size)];

        fixed (byte* pResult = result)
        {
            Buffer.MemoryCopy(pBuffer, pResult, size, size);
        }

        return result;
#else
        return new Span<byte>(pBuffer, (int)size);
#endif
    }
}
