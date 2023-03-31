// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-16.0.6/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CXTYPE_H
#define LIBCLANGSHARP_CXTYPE_H

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

#include <clang-c/Index.h>
#include <clang/AST/Type.h>

#pragma warning(pop)

clang::QualType GetQualType(CXType CT);
CXTranslationUnit GetTypeTU(CXType CT);

namespace clang::cxtype {
    CXType MakeCXType(QualType T, CXTranslationUnit TU);
}

#endif
