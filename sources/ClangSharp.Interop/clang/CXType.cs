// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-17.0.4/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

using System.Runtime.CompilerServices;

namespace ClangSharp.Interop;

public partial struct CXType
{
    [NativeTypeName("enum CXTypeKind")]
    public CXTypeKind kind;

    [NativeTypeName("void *[2]")]
    public _data_e__FixedBuffer data;

    public unsafe partial struct _data_e__FixedBuffer
    {
        public void* e0;
        public void* e1;

        public ref void* this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (void** pThis = &e0)
                {
                    return ref pThis[index];
                }
            }
        }
    }
}
