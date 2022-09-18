// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-15.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXIdxEntityLanguage
{
    CXIdxEntityLang_None = 0,
    CXIdxEntityLang_C = 1,
    CXIdxEntityLang_ObjC = 2,
    CXIdxEntityLang_CXX = 3,
    CXIdxEntityLang_Swift = 4,
}
