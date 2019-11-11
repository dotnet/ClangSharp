// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
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
}
