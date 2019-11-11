// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxObjCInterfaceDeclInfo
    {
        [NativeTypeName("const CXIdxObjCContainerDeclInfo *")]
        public CXIdxObjCContainerDeclInfo* containerInfo;

        [NativeTypeName("const CXIdxBaseClassInfo *")]
        public CXIdxBaseClassInfo* superInfo;

        [NativeTypeName("const CXIdxObjCProtocolRefListInfo *")]
        public CXIdxObjCProtocolRefListInfo* protocols;
    }
}
