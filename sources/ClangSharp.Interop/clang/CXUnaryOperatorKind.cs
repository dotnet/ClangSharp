// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-17.0.4/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXUnaryOperatorKind
{
    CXUnaryOperator_Invalid,
    CXUnaryOperator_PostInc,
    CXUnaryOperator_PostDec,
    CXUnaryOperator_PreInc,
    CXUnaryOperator_PreDec,
    CXUnaryOperator_AddrOf,
    CXUnaryOperator_Deref,
    CXUnaryOperator_Plus,
    CXUnaryOperator_Minus,
    CXUnaryOperator_Not,
    CXUnaryOperator_LNot,
    CXUnaryOperator_Real,
    CXUnaryOperator_Imag,
    CXUnaryOperator_Extension,
    CXUnaryOperator_Coawait,
}
