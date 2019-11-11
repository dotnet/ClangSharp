// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxIncludedFileInfo
    {
        public CXIdxLoc hashLoc;

        [NativeTypeName("const char *")]
        public sbyte* filename;

        [NativeTypeName("CXFile")]
        public void* file;

        public int isImport;

        public int isAngled;

        public int isModuleImport;
    }
}
