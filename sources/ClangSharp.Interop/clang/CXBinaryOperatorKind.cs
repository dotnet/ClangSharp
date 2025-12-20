// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXBinaryOperatorKind
{
    CXBinaryOperator_Invalid = 0,
    CXBinaryOperator_PtrMemD = 1,
    CXBinaryOperator_PtrMemI = 2,
    CXBinaryOperator_Mul = 3,
    CXBinaryOperator_Div = 4,
    CXBinaryOperator_Rem = 5,
    CXBinaryOperator_Add = 6,
    CXBinaryOperator_Sub = 7,
    CXBinaryOperator_Shl = 8,
    CXBinaryOperator_Shr = 9,
    CXBinaryOperator_Cmp = 10,
    CXBinaryOperator_LT = 11,
    CXBinaryOperator_GT = 12,
    CXBinaryOperator_LE = 13,
    CXBinaryOperator_GE = 14,
    CXBinaryOperator_EQ = 15,
    CXBinaryOperator_NE = 16,
    CXBinaryOperator_And = 17,
    CXBinaryOperator_Xor = 18,
    CXBinaryOperator_Or = 19,
    CXBinaryOperator_LAnd = 20,
    CXBinaryOperator_LOr = 21,
    CXBinaryOperator_Assign = 22,
    CXBinaryOperator_MulAssign = 23,
    CXBinaryOperator_DivAssign = 24,
    CXBinaryOperator_RemAssign = 25,
    CXBinaryOperator_AddAssign = 26,
    CXBinaryOperator_SubAssign = 27,
    CXBinaryOperator_ShlAssign = 28,
    CXBinaryOperator_ShrAssign = 29,
    CXBinaryOperator_AndAssign = 30,
    CXBinaryOperator_XorAssign = 31,
    CXBinaryOperator_OrAssign = 32,
    CXBinaryOperator_Comma = 33,
    CXBinaryOperator_Last = CXBinaryOperator_Comma,
}
