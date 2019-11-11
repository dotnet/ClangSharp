// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
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
}
