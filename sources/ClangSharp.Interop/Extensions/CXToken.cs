// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXToken : IEquatable<CXToken>
{
    public readonly CXTokenKind Kind => clang.getTokenKind(this);

    public static bool operator ==(CXToken left, CXToken right)
    {
        return (left.int_data[0] == right.int_data[0]) &&
               (left.int_data[1] == right.int_data[1]) &&
               (left.int_data[2] == right.int_data[2]) &&
               (left.int_data[3] == right.int_data[3]) &&
               (left.ptr_data == right.ptr_data);
    }

    public static bool operator !=(CXToken left, CXToken right)
    {
        return (left.int_data[0] != right.int_data[0]) ||
               (left.int_data[1] != right.int_data[1]) ||
               (left.int_data[2] != right.int_data[2]) ||
               (left.int_data[3] != right.int_data[3]) ||
               (left.ptr_data != right.ptr_data);
    }

    public override readonly bool Equals(object? obj) => (obj is CXSourceRange other) && Equals(other);

    public readonly bool Equals(CXToken other) => this == other;

    public readonly CXSourceRange GetExtent(CXTranslationUnit translationUnit) => clang.getTokenExtent(translationUnit, this);

    public override readonly int GetHashCode() => HashCode.Combine(int_data[0], int_data[1], int_data[2], int_data[3], (IntPtr)ptr_data);

    public readonly CXSourceLocation GetLocation(CXTranslationUnit translationUnit) => clang.getTokenLocation(translationUnit, this);

    public readonly CXString GetSpelling(CXTranslationUnit translationUnit) => clang.getTokenSpelling(translationUnit, this);

    public override readonly string ToString() => "";
}
