// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-16.0.6/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxDeclInfo
{
    [NativeTypeName("const CXIdxEntityInfo *")]
    public CXIdxEntityInfo* entityInfo;

    public CXCursor cursor;

    public CXIdxLoc loc;

    [NativeTypeName("const CXIdxContainerInfo *")]
    public CXIdxContainerInfo* semanticContainer;

    [NativeTypeName("const CXIdxContainerInfo *")]
    public CXIdxContainerInfo* lexicalContainer;

    public int isRedeclaration;

    public int isDefinition;

    public int isContainer;

    [NativeTypeName("const CXIdxContainerInfo *")]
    public CXIdxContainerInfo* declAsContainer;

    public int isImplicit;

    [NativeTypeName("const CXIdxAttrInfo *const *")]
    public CXIdxAttrInfo** attributes;

    [NativeTypeName("unsigned int")]
    public uint numAttributes;

    [NativeTypeName("unsigned int")]
    public uint flags;
}
