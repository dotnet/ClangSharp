// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXIndex : IDisposable, IEquatable<CXIndex>
{
    public CXIndex(IntPtr handle)
    {
        Handle = handle;
    }

    public readonly CXGlobalOptFlags GlobalOptions
    {
        get
        {
            return (CXGlobalOptFlags)clang.CXIndex_getGlobalOptions(this);
        }

        set
        {
            clang.CXIndex_setGlobalOptions(this, (uint)value);
        }
    }

    public IntPtr Handle { get; set; }

    public static explicit operator CXIndex(void* value) => new CXIndex((IntPtr)value);

    public static implicit operator void*(CXIndex value) => (void*)value.Handle;

    public static bool operator ==(CXIndex left, CXIndex right) => left.Handle == right.Handle;

    public static bool operator !=(CXIndex left, CXIndex right) => left.Handle != right.Handle;

    public static CXIndex Create(bool excludeDeclarationsFromPch = false, bool displayDiagnostics = false) => (CXIndex)clang.createIndex(excludeDeclarationsFromPch ? 1 : 0, displayDiagnostics ? 1 : 0);

    public readonly void Dispose() => clang.disposeIndex(this);

    public override readonly bool Equals(object? obj) => (obj is CXIndex other) && Equals(other);

    public readonly bool Equals(CXIndex other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly void SetInvocationEmissionPathOption(string Path)
    {
        using var marshaledPath = new MarshaledString(Path);
        clang.CXIndex_setInvocationEmissionPathOption(this, marshaledPath);
    }
}
