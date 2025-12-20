// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System.Runtime.CompilerServices;

namespace ClangSharp.Interop;

public unsafe partial struct CXToken
{
    [NativeTypeName("unsigned int[4]")]
    public _int_data_e__FixedBuffer int_data;

    public void* ptr_data;

    [InlineArray(4)]
    public partial struct _int_data_e__FixedBuffer
    {
        public uint e0;
    }
}
