// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-18.1.3/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System.Runtime.CompilerServices;

namespace ClangSharp.Interop;

public partial struct CXFileUniqueID
{
    [NativeTypeName("unsigned long long[3]")]
    public _data_e__FixedBuffer data;

    [InlineArray(3)]
    public partial struct _data_e__FixedBuffer
    {
        public ulong e0;
    }
}
