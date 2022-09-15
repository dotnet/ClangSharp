// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxLoc
{
    public CXSourceLocation SourceLocation => clang.indexLoc_getCXSourceLocation(this);

    public void GetFileLocation(out CXIdxClientFile indexFile, out CXFile file, out uint line, out uint column, out uint offset)
    {
        fixed (CXIdxClientFile* pIndexFile = &indexFile)
        fixed (CXFile* pFile = &file)
        fixed (uint* pLine = &line)
        fixed (uint* pColumn = &column)
        fixed (uint* pOffset = &offset)
        {
            clang.indexLoc_getFileLocation(this, (void**)pIndexFile, (void**)pFile, pLine, pColumn, pOffset);
        }
    }
}
