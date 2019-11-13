// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-9.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CXSOURCELOCATION_H
#define LIBCLANGSHARP_CXSOURCELOCATION_H

#include <clang/Basic/Diagnostic.h>
#include <clang/Basic/SourceLocation.h>
#include <clang/Basic/SourceManager.h>
#include <clang-c/Index.h>

void createNullLocation(CXFile* file, unsigned* line, unsigned* column, unsigned* offset);

clang::SourceRange getRawCursorExtent(CXCursor C);

bool isASTUnitSourceLocation(const CXSourceLocation& L);

namespace clang::cxloc {
    CXSourceRange translateSourceRange(ASTContext& Context, SourceRange R);
    CXSourceRange translateSourceRange(const SourceManager& SM, const LangOptions& LangOpts, const CharSourceRange& R);
}

#endif
