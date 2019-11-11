// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxImportedASTFileInfo
    {
        [NativeTypeName("CXFile")]
        public void* file;

        [NativeTypeName("CXModule")]
        public void* module;

        public CXIdxLoc loc;

        public int isImplicit;
    }
}
