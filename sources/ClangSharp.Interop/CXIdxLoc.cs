// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-14.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public partial struct CXIdxLoc
{
    [NativeTypeName("void *[2]")]
    public _ptr_data_e__FixedBuffer ptr_data;

    [NativeTypeName("unsigned int")]
    public uint int_data;

    public unsafe partial struct _ptr_data_e__FixedBuffer
    {
        public void* e0;
        public void* e1;

        public ref void* this[int index]
        {
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
