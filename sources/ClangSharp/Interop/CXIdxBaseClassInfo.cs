// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxBaseClassInfo
    {
        [NativeTypeName("const CXIdxEntityInfo *")]
        public CXIdxEntityInfo* @base;

        public CXCursor cursor;

        public CXIdxLoc loc;
    }
}
