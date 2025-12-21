// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxObjCPropertyDeclInfo
{
    [NativeTypeName("const CXIdxDeclInfo *")]
    public CXIdxDeclInfo* declInfo;

    [NativeTypeName("const CXIdxEntityInfo *")]
    public CXIdxEntityInfo* getter;

    [NativeTypeName("const CXIdxEntityInfo *")]
    public CXIdxEntityInfo* setter;
}
