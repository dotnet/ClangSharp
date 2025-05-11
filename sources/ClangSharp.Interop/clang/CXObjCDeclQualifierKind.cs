// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-20.1.2/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXObjCDeclQualifierKind
{
    CXObjCDeclQualifier_None = 0x0,
    CXObjCDeclQualifier_In = 0x1,
    CXObjCDeclQualifier_Inout = 0x2,
    CXObjCDeclQualifier_Out = 0x4,
    CXObjCDeclQualifier_Bycopy = 0x8,
    CXObjCDeclQualifier_Byref = 0x10,
    CXObjCDeclQualifier_Oneway = 0x20,
}
