// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-18.1.3/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxEntityRefInfo
{
    public CXIdxEntityRefKind kind;

    public CXCursor cursor;

    public CXIdxLoc loc;

    [NativeTypeName("const CXIdxEntityInfo *")]
    public CXIdxEntityInfo* referencedEntity;

    [NativeTypeName("const CXIdxEntityInfo *")]
    public CXIdxEntityInfo* parentEntity;

    [NativeTypeName("const CXIdxContainerInfo *")]
    public CXIdxContainerInfo* container;

    public CXSymbolRole role;
}
