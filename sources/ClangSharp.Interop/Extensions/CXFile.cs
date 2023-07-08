// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXFile : IEquatable<CXFile>
{
    public CXFile(IntPtr handle)
    {
        Handle = handle;
    }

    public IntPtr Handle { get; set; }

    public readonly CXString Name => clang.getFileName(this);

    public readonly long Time => clang.getFileTime(this);

    public static explicit operator CXFile(void* value) => new CXFile((IntPtr)value);

    public static implicit operator void*(CXFile value) => (void*)value.Handle;

    public static bool operator ==(CXFile left, CXFile right) => clang.File_isEqual(left, right) != 0;

    public static bool operator !=(CXFile left, CXFile right) => clang.File_isEqual(left, right) == 0;

    public override readonly bool Equals(object? obj) => (obj is CXFile other) && Equals(other);

    public readonly bool Equals(CXFile other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public override readonly string ToString() => Name.ToString();

    public readonly bool TryGetUniqueId(out CXFileUniqueID id)
    {
        fixed (CXFileUniqueID* pId = &id)
        {
            return clang.getFileUniqueID(this, pId) == 0;
        }
    }

    public readonly CXString TryGetRealPathName() => clang.File_tryGetRealPathName(this);
}
