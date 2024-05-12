// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXTargetInfo(IntPtr handle) : IDisposable, IEquatable<CXTargetInfo>
{
    public IntPtr Handle { get; set; } = handle;

    public readonly int PointerWidth => clang.TargetInfo_getPointerWidth(this);

    public readonly CXString Triple => clang.TargetInfo_getTriple(this);

    public static implicit operator CXTargetInfo(CXTargetInfoImpl* value) => new CXTargetInfo((IntPtr)value);

    public static implicit operator CXTargetInfoImpl*(CXTargetInfo value) => (CXTargetInfoImpl*)value.Handle;

    public static bool operator ==(CXTargetInfo left, CXTargetInfo right) => left.Handle == right.Handle;

    public static bool operator !=(CXTargetInfo left, CXTargetInfo right) => left.Handle != right.Handle;

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            clang.TargetInfo_dispose(this);
            Handle = IntPtr.Zero;
        }
    }

    public override readonly bool Equals(object? obj) => (obj is CXTargetInfo other) && Equals(other);

    public readonly bool Equals(CXTargetInfo other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public override readonly string ToString() => Triple.ToString();
}
