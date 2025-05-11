// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-20.1.2/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXBinaryOperatorKind
{
    CXBinaryOperator_Invalid,
    CXBinaryOperator_PtrMemD,
    CXBinaryOperator_PtrMemI,
    CXBinaryOperator_Mul,
    CXBinaryOperator_Div,
    CXBinaryOperator_Rem,
    CXBinaryOperator_Add,
    CXBinaryOperator_Sub,
    CXBinaryOperator_Shl,
    CXBinaryOperator_Shr,
    CXBinaryOperator_Cmp,
    CXBinaryOperator_LT,
    CXBinaryOperator_GT,
    CXBinaryOperator_LE,
    CXBinaryOperator_GE,
    CXBinaryOperator_EQ,
    CXBinaryOperator_NE,
    CXBinaryOperator_And,
    CXBinaryOperator_Xor,
    CXBinaryOperator_Or,
    CXBinaryOperator_LAnd,
    CXBinaryOperator_LOr,
    CXBinaryOperator_Assign,
    CXBinaryOperator_MulAssign,
    CXBinaryOperator_DivAssign,
    CXBinaryOperator_RemAssign,
    CXBinaryOperator_AddAssign,
    CXBinaryOperator_SubAssign,
    CXBinaryOperator_ShlAssign,
    CXBinaryOperator_ShrAssign,
    CXBinaryOperator_AndAssign,
    CXBinaryOperator_XorAssign,
    CXBinaryOperator_OrAssign,
    CXBinaryOperator_Comma,
}
