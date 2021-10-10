// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "ClangSharp.h"
#include "CXCursor.h"
#include "CXSourceLocation.h"

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

#include <clang/Basic/SourceLocation.h>
#include <clang/Frontend/ASTUnit.h>
#include <clang/Lex/Lexer.h>

#pragma warning(pop)

void createNullLocation(CXFile* file, unsigned* line, unsigned* column, unsigned* offset) {
    if (file)
        *file = nullptr;
    if (line)
        *line = 0;
    if (column)
        *column = 0;
    if (offset)
        *offset = 0;
}

clang::SourceRange getCursorSourceRange(CXCursor C) {
    using namespace clang;
    using namespace clang::cxcursor;

    if (clang_isAttribute(C.kind))
        return getCursorAttr(C)->getRange();

    if (clang_isDeclaration(C.kind) || clang_isTranslationUnit(C.kind))
        return getCursorDecl(C)->getSourceRange();

    if (clang_isExpression(C.kind) || clang_isStatement(C.kind))
        return getCursorStmt(C)->getSourceRange();

    if (clang_isPreprocessing(C.kind))
        return getCursorPreprocessedEntity(C)->getSourceRange();

    return SourceRange();
}

bool isASTUnitSourceLocation(const CXSourceLocation& L) {
    // If the lowest bit is clear then the first ptr_data entry is a SourceManager
    // pointer, or the CXSourceLocation is a null location.
    return ((uintptr_t)L.ptr_data[0] & 0x1) == 0;
}

namespace clang::cxloc {
    CXSourceLocation translateSourceLocation(ASTContext& Context, SourceLocation Loc) {
        return translateSourceLocation(Context.getSourceManager(), Context.getLangOpts(), Loc);
    }
    CXSourceLocation translateSourceLocation(const SourceManager& SM, const LangOptions& LangOpts, SourceLocation Loc) {
        if (Loc.isInvalid())
            return clang_getNullLocation();

        CXSourceLocation Result = {
            { &SM, &LangOpts, },
            Loc.getRawEncoding()
        };
        return Result;
    }

    CXSourceRange translateSourceRange(ASTContext& Context, SourceRange R) {
        return translateSourceRange(Context.getSourceManager(), Context.getLangOpts(), R);
    }

    CXSourceRange translateSourceRange(const SourceManager& SM, const LangOptions& LangOpts, SourceRange R) {
        CXSourceRange Result = {
            { &SM, &LangOpts },
            R.getBegin().getRawEncoding(),
            R.getEnd().getRawEncoding()
        };
        return Result;
    }
}
