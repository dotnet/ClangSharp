// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-17.0.4/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXEvalResultKind
{
    CXEval_Int = 1,
    CXEval_Float = 2,
    CXEval_ObjCStrLiteral = 3,
    CXEval_StrLiteral = 4,
    CXEval_CFStr = 5,
    CXEval_Other = 6,
    CXEval_UnExposed = 0,
}
