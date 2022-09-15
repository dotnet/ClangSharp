// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxAttrInfo
{
    public CXIdxIBOutletCollectionAttrInfo* IBOutletCollectionAttrInfo
    {
        get
        {
            fixed (CXIdxAttrInfo* pThis = &this)
            {
                return clang.index_getIBOutletCollectionAttrInfo(pThis);
            }
        }
    }
}
