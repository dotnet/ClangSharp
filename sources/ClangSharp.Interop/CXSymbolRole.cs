// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-14.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXSymbolRole
{
    CXSymbolRole_None = 0,
    CXSymbolRole_Declaration = 1 << 0,
    CXSymbolRole_Definition = 1 << 1,
    CXSymbolRole_Reference = 1 << 2,
    CXSymbolRole_Read = 1 << 3,
    CXSymbolRole_Write = 1 << 4,
    CXSymbolRole_Call = 1 << 5,
    CXSymbolRole_Dynamic = 1 << 6,
    CXSymbolRole_AddressOf = 1 << 7,
    CXSymbolRole_Implicit = 1 << 8,
}
