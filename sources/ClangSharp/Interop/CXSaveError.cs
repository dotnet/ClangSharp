// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-11.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXSaveError
    {
        CXSaveError_None = 0,
        CXSaveError_Unknown = 1,
        CXSaveError_TranslationErrors = 2,
        CXSaveError_InvalidTU = 3,
    }
}
