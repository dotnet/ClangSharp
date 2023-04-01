// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-16.0.6/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CXSOURCELOCATION_H
#define LIBCLANGSHARP_CXSOURCELOCATION_H

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

#include <clang-c/Index.h>
#include <clang/AST/ASTContext.h>
#include <clang/Basic/LangOptions.h>
#include <clang/Basic/SourceLocation.h>

#pragma warning(pop)

void createNullLocation(CXFile* file, unsigned* line, unsigned* column, unsigned* offset);

clang::SourceRange getCursorSourceRange(CXCursor C);

bool isASTUnitSourceLocation(const CXSourceLocation& L);

namespace clang::cxloc {
    CXSourceLocation translateSourceLocation(ASTContext& Context, SourceLocation Loc);
    CXSourceLocation translateSourceLocation(const SourceManager& SM, const LangOptions& LangOpts, SourceLocation Loc);

    CXSourceRange translateSourceRange(ASTContext& Context, SourceRange R);
    CXSourceRange translateSourceRangeRaw(ASTContext& Context, SourceRange R);

    CXSourceRange translateSourceRange(const SourceManager& SM, const LangOptions& LangOpts, const CharSourceRange& R);
    CXSourceRange translateSourceRangeRaw(const SourceManager& SM, const LangOptions& LangOpts, const CharSourceRange& R);
}

#endif
