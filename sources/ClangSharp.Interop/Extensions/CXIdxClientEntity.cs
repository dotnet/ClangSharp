// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxClientEntity : IEquatable<CXIdxClientEntity>
{
    public CXIdxClientEntity(IntPtr handle)
    {
        Handle = handle;
    }

    public IntPtr Handle { get; set; }

    public static explicit operator CXIdxClientEntity(void* value) => new CXIdxClientEntity((IntPtr)value);

    public static implicit operator void*(CXIdxClientEntity value) => (void*)value.Handle;

    public static bool operator ==(CXIdxClientEntity left, CXIdxClientEntity right) => left.Handle == right.Handle;

    public static bool operator !=(CXIdxClientEntity left, CXIdxClientEntity right) => left.Handle != right.Handle;

    public override readonly bool Equals(object? obj) => (obj is CXIdxClientEntity other) && Equals(other);

    public readonly bool Equals(CXIdxClientEntity other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();
}
