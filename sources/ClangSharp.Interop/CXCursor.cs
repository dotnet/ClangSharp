// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public partial struct CXCursor
    {
        [NativeTypeName("enum CXCursorKind")]
        public CXCursorKind kind;

        public int xdata;

        [NativeTypeName("const void *[3]")]
        public _data_e__FixedBuffer data;

        public unsafe partial struct _data_e__FixedBuffer
        {
            public void* e0;
            public void* e1;
            public void* e2;

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
}
