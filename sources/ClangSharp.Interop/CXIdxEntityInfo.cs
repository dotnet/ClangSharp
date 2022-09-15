// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-14.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public unsafe partial struct CXIdxEntityInfo
{
    public CXIdxEntityKind kind;

    public CXIdxEntityCXXTemplateKind templateKind;

    public CXIdxEntityLanguage lang;

    [NativeTypeName("const char *")]
    public sbyte* name;

    [NativeTypeName("const char *")]
    public sbyte* USR;

    public CXCursor cursor;

    [NativeTypeName("const CXIdxAttrInfo *const *")]
    public CXIdxAttrInfo** attributes;

    [NativeTypeName("unsigned int")]
    public uint numAttributes;
}
