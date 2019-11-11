// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxEntityRefInfo
    {
        public CXIdxEntityRefKind kind;

        public CXCursor cursor;

        public CXIdxLoc loc;

        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* referencedEntity;

        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* parentEntity;

        [NativeTypeName("const CXIdxContainerInfo *")]
        public CXIdxContainerInfo* container;

        public CXSymbolRole role;
    }
}
