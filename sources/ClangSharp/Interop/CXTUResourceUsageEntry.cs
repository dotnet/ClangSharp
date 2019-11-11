// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

#if Windows_NT
using nulong = System.UInt32;
#else
using System;
using nulong = System.UIntPtr;
#endif

namespace ClangSharp.Interop
{
    public partial struct CXTUResourceUsageEntry
    {
        [NativeTypeName("enum CXTUResourceUsageKind")]
        public CXTUResourceUsageKind kind;

        [NativeTypeName("unsigned long")]
        public nulong amount;
    }
}
