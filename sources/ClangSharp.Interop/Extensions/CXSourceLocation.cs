// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CXSourceLocation : IEquatable<CXSourceLocation>
{
    public static CXSourceLocation Null => clang.getNullLocation();

    public readonly bool IsFromMainFile => clang.Location_isFromMainFile(this) != 0;

    public readonly bool IsInSystemHeader => clang.Location_isInSystemHeader(this) != 0;

    public static bool operator ==(CXSourceLocation left, CXSourceLocation right) => clang.equalLocations(left, right) != 0;

    public static bool operator !=(CXSourceLocation left, CXSourceLocation right) => clang.equalLocations(left, right) == 0;

    public override readonly bool Equals(object? obj) => (obj is CXSourceLocation other) && Equals(other);

    public readonly bool Equals(CXSourceLocation other) => this == other;

    public readonly void GetExpansionLocation(out CXFile file, out uint line, out uint column, out uint offset)
    {
        fixed (CXFile* pFile = &file)
        fixed (uint* pLine = &line)
        fixed (uint* pColumn = &column)
        fixed (uint* pOffset = &offset)
        {
            clang.getExpansionLocation(this, (void**)pFile, pLine, pColumn, pOffset);
        }
    }

    public readonly void GetFileLocation(out CXFile file, out uint line, out uint column, out uint offset)
    {
        fixed (CXFile* pFile = &file)
        fixed (uint* pLine = &line)
        fixed (uint* pColumn = &column)
        fixed (uint* pOffset = &offset)
        {
            clang.getFileLocation(this, (void**)pFile, pLine, pColumn, pOffset);
        }
    }

    public override readonly int GetHashCode() => HashCode.Combine(ptr_data, int_data);

    public readonly void GetInstantiationLocation(out CXFile file, out uint line, out uint column, out uint offset)
    {
        fixed (CXFile* pFile = &file)
        fixed (uint* pLine = &line)
        fixed (uint* pColumn = &column)
        fixed (uint* pOffset = &offset)
        {
            clang.getInstantiationLocation(this, (void**)pFile, pLine, pColumn, pOffset);
        }
    }

    public readonly void GetPresumedLocation(out CXString fileName, out uint line, out uint column)
    {
        fixed (CXString* pFileName = &fileName)
        fixed (uint* pLine = &line)
        fixed (uint* pColumn = &column)
        {
            clang.getPresumedLocation(this, pFileName, pLine, pColumn);
        }
    }

    public readonly void GetSpellingLocation(out CXFile file, out uint line, out uint column, out uint offset)
    {
        fixed (CXFile* pFile = &file)
        fixed (uint* pLine = &line)
        fixed (uint* pColumn = &column)
        fixed (uint* pOffset = &offset)
        {
            clang.getSpellingLocation(this, (void**)pFile, pLine, pColumn, pOffset);
        }
    }

    public override readonly string ToString()
    {
        GetSpellingLocation(out var file, out var line, out var column, out _);
        return $"Line {line}, Column {column} in {file}";
    }
}
