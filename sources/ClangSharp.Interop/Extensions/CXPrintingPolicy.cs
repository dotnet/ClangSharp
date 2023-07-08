// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXPrintingPolicy : IDisposable, IEquatable<CXPrintingPolicy>
{
    public CXPrintingPolicy(IntPtr handle)
    {
        Handle = handle;
    }

    public IntPtr Handle { get; set; }

    public static explicit operator CXPrintingPolicy(void* value) => new CXPrintingPolicy((IntPtr)value);

    public static implicit operator void*(CXPrintingPolicy value) => (void*)value.Handle;

    public static bool operator ==(CXPrintingPolicy left, CXPrintingPolicy right) => left.Handle == right.Handle;

    public static bool operator !=(CXPrintingPolicy left, CXPrintingPolicy right) => left.Handle != right.Handle;

    public void Dispose()
    {
        if (Handle != IntPtr.Zero)
        {
            clang.PrintingPolicy_dispose(this);
            Handle = IntPtr.Zero;
        }
    }

    public override readonly bool Equals(object? obj) => (obj is CXPrintingPolicy other) && Equals(other);

    public readonly bool Equals(CXPrintingPolicy other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly uint GetProperty(CXPrintingPolicyProperty property) => clang.PrintingPolicy_getProperty(this, property);

    public readonly void SetProperty(CXPrintingPolicyProperty property, uint value) => clang.PrintingPolicy_setProperty(this, property, value);
}
