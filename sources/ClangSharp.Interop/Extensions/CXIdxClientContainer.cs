// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxClientContainer : IEquatable<CXIdxClientContainer>
{
    public CXIdxClientContainer(IntPtr handle)
    {
        Handle = handle;
    }

    public IntPtr Handle { get; set; }

    public static explicit operator CXIdxClientContainer(void* value) => new CXIdxClientContainer((IntPtr)value);

    public static implicit operator void*(CXIdxClientContainer value) => (void*)value.Handle;

    public static bool operator ==(CXIdxClientContainer left, CXIdxClientContainer right) => left.Handle == right.Handle;

    public static bool operator !=(CXIdxClientContainer left, CXIdxClientContainer right) => left.Handle != right.Handle;

    public override readonly bool Equals(object? obj) => (obj is CXIdxClientContainer other) && Equals(other);

    public readonly bool Equals(CXIdxClientContainer other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();
}
