// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-16.0.6/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "CXTranslationUnit.h"

namespace clang::cxtu {
    ASTUnit* getASTUnit(CXTranslationUnit TU) {
        if (!TU) {
            return nullptr;
        }
        return TU->TheASTUnit;
    }
}
