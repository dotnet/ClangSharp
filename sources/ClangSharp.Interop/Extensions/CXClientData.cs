// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXClientData : IEquatable<CXClientData>
{
    public CXClientData(IntPtr handle)
    {
        Handle = handle;
    }

    public IntPtr Handle { get; set; }

    public static explicit operator CXClientData(void* value) => new CXClientData((IntPtr)value);

    public static implicit operator void*(CXClientData value) => (void*)value.Handle;

    public static bool operator ==(CXClientData left, CXClientData right) => left.Handle == right.Handle;

    public static bool operator !=(CXClientData left, CXClientData right) => left.Handle != right.Handle;

    public override bool Equals(object obj) => (obj is CXClientData other) && Equals(other);

    public bool Equals(CXClientData other) => this == other;

    public override int GetHashCode() => Handle.GetHashCode();
}
