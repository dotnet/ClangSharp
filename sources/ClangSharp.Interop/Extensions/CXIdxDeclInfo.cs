// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxDeclInfo
    {
        public CXIdxCXXClassDeclInfo* CXXClassDeclInfo
        {
            get
            {
                fixed (CXIdxDeclInfo* pThis = &this)
                {
                    return clang.index_getCXXClassDeclInfo(pThis);
                }
            }
        }

        public CXIdxObjCCategoryDeclInfo* ObjCCategoryDeclInfo
        {
            get
            {
                fixed (CXIdxDeclInfo* pThis = &this)
                {
                    return clang.index_getObjCCategoryDeclInfo(pThis);
                }
            }
        }

        public CXIdxObjCContainerDeclInfo* ObjCContainerDeclInfo
        {
            get
            {
                fixed (CXIdxDeclInfo* pThis = &this)
                {
                    return clang.index_getObjCContainerDeclInfo(pThis);
                }
            }
        }

        public CXIdxObjCInterfaceDeclInfo* ObjCInterfaceDeclInfo
        {
            get
            {
                fixed (CXIdxDeclInfo* pThis = &this)
                {
                    return clang.index_getObjCInterfaceDeclInfo(pThis);
                }
            }
        }

        public CXIdxObjCPropertyDeclInfo* ObjCPropertyDeclInfo
        {
            get
            {
                fixed (CXIdxDeclInfo* pThis = &this)
                {
                    return clang.index_getObjCPropertyDeclInfo(pThis);
                }
            }
        }

        public CXIdxObjCProtocolRefListInfo* ObjCProtocolRefListInfo
        {
            get
            {
                fixed (CXIdxDeclInfo* pThis = &this)
                {
                    return clang.index_getObjCProtocolRefListInfo(pThis);
                }
            }
        }
    }
}
