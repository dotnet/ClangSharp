// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxClientASTFile(IntPtr handle) : IEquatable<CXIdxClientASTFile>
{
    public IntPtr Handle { get; set; } = handle;

    public static explicit operator CXIdxClientASTFile(void* value) => new CXIdxClientASTFile((IntPtr)value);

    public static implicit operator void*(CXIdxClientASTFile value) => (void*)value.Handle;

    public static bool operator ==(CXIdxClientASTFile left, CXIdxClientASTFile right) => left.Handle == right.Handle;

    public static bool operator !=(CXIdxClientASTFile left, CXIdxClientASTFile right) => left.Handle != right.Handle;

    public override readonly bool Equals(object? obj) => (obj is CXIdxClientASTFile other) && Equals(other);

    public readonly bool Equals(CXIdxClientASTFile other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();
}
