// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXModule : IEquatable<CXModule>
{
    public CXModule(IntPtr handle)
    {
        Handle = handle;
    }

    public readonly CXFile AstFile => (CXFile)clang.Module_getASTFile(this);

    public readonly CXString FullName => clang.Module_getFullName(this);

    public IntPtr Handle { get; set; }

    public readonly bool IsSystem => clang.Module_isSystem(this) != 0;

    public readonly CXString Name => clang.Module_getName(this);

    public readonly CXModule Parent => (CXModule)clang.Module_getParent(this);

    public static explicit operator CXModule(void* value) => new CXModule((IntPtr)value);

    public static implicit operator void*(CXModule value) => (void*)value.Handle;

    public static bool operator ==(CXModule left, CXModule right) => left.Handle == right.Handle;

    public static bool operator !=(CXModule left, CXModule right) => left.Handle != right.Handle;

    public override readonly bool Equals(object? obj) => (obj is CXModule other) && Equals(other);

    public readonly bool Equals(CXModule other) => this == other;

    public override readonly int GetHashCode() => Handle.GetHashCode();

    public readonly uint GetNumTopLevelHeaders(CXTranslationUnit translationUnit) => clang.Module_getNumTopLevelHeaders(translationUnit, this);

    public readonly CXFile GetTopLevelHeader(CXTranslationUnit translationUnit, uint index) => (CXFile)clang.Module_getTopLevelHeader(translationUnit, this, index);

    public override readonly string ToString() => FullName.ToString();
}
