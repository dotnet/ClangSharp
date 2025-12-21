// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CX_BinaryOperatorKind
{
    CX_BO_Invalid = 0,
    CX_BO_PtrMemD = 1,
    CX_BO_PtrMemI = 2,
    CX_BO_Mul = 3,
    CX_BO_Div = 4,
    CX_BO_Rem = 5,
    CX_BO_Add = 6,
    CX_BO_Sub = 7,
    CX_BO_Shl = 8,
    CX_BO_Shr = 9,
    CX_BO_Cmp = 10,
    CX_BO_LT = 11,
    CX_BO_GT = 12,
    CX_BO_LE = 13,
    CX_BO_GE = 14,
    CX_BO_EQ = 15,
    CX_BO_NE = 16,
    CX_BO_And = 17,
    CX_BO_Xor = 18,
    CX_BO_Or = 19,
    CX_BO_LAnd = 20,
    CX_BO_LOr = 21,
    CX_BO_Assign = 22,
    CX_BO_MulAssign = 23,
    CX_BO_DivAssign = 24,
    CX_BO_RemAssign = 25,
    CX_BO_AddAssign = 26,
    CX_BO_SubAssign = 27,
    CX_BO_ShlAssign = 28,
    CX_BO_ShrAssign = 29,
    CX_BO_AndAssign = 30,
    CX_BO_XorAssign = 31,
    CX_BO_OrAssign = 32,
    CX_BO_Comma = 33,
    CX_BO_LAST = CX_BO_Comma,
}
