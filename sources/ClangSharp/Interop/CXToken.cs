// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public unsafe partial struct CXToken
    {
        [NativeTypeName("unsigned int [4]")]
        public fixed uint int_data[4];

        [NativeTypeName("void *")]
        public void* ptr_data;
    }
}
