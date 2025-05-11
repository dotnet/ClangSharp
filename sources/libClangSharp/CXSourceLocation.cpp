// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-20.1.2/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "ClangSharp.h"

#include "CXCursor.h"
#include "CXSourceLocation.h"

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

// #include <clang/Frontend/ASTUnit.h>
// #include <clang/Lex/Lexer.h>

#pragma warning(pop)

void createNullLocation(CXFile* file, unsigned* line, unsigned* column, unsigned* offset) {
    if (file) {
        *file = nullptr;
    }

    if (line) {
        *line = 0;
    }

    if (column) {
        *column = 0;
    }

    if (offset) {
        *offset = 0;
    }
}

clang::SourceRange getCursorSourceRange(CXCursor C) {
    using namespace clang;
    using namespace clang::cxcursor;

    if (clang_isAttribute(C.kind)) {
        return getCursorAttr(C)->getRange();
    }

    if (clang_isDeclaration(C.kind) || clang_isTranslationUnit(C.kind)) {
        return getCursorDecl(C)->getSourceRange();
    }

    if (clang_isExpression(C.kind) || clang_isStatement(C.kind)) {
        return getCursorStmt(C)->getSourceRange();
    }

    if (clang_isPreprocessing(C.kind)) {
        return getCursorPreprocessedEntity(C)->getSourceRange();
    }

    return SourceRange();
}

bool isASTUnitSourceLocation(const CXSourceLocation& L) {
    // If the lowest bit is clear then the first ptr_data entry is a SourceManager
    // pointer, or the CXSourceLocation is a null location.
    return ((uintptr_t)L.ptr_data[0] & 0x1) == 0;
}

namespace clang::cxloc {
    /// Translate a Clang source location into a CIndex source location.
    CXSourceLocation translateSourceLocation(ASTContext& Context, SourceLocation Loc) {
        return translateSourceLocation(Context.getSourceManager(), Context.getLangOpts(), Loc);
    }

    /// Translate a Clang source location into a CIndex source location.
    CXSourceLocation translateSourceLocation(const SourceManager& SM, const LangOptions& LangOpts, SourceLocation Loc) {
        if (Loc.isInvalid()) {
            return clang_getNullLocation();
        }

        CXSourceLocation Result = {
            {
                &SM,
                &LangOpts,
            },
            Loc.getRawEncoding()
        };
        return Result;
    }

    /// Translate a Clang source range into a CIndex source range.
    CXSourceRange translateSourceRange(ASTContext& Context, SourceRange R) {
        return translateSourceRange(Context.getSourceManager(), Context.getLangOpts(), CharSourceRange::getTokenRange(R));
    }

    CXSourceRange translateSourceRangeRaw(ASTContext& Context, SourceRange R) {
        return translateSourceRangeRaw(Context.getSourceManager(), Context.getLangOpts(), CharSourceRange::getTokenRange(R));
    }

    /// Translate a Clang source range into a CIndex source range.
    ///
    /// Clang internally represents ranges where the end location points to the
    /// start of the token at the end. However, for external clients it is more
    /// useful to have a CXSourceRange be a proper half-open interval. This routine
    /// does the appropriate translation.
    CXSourceRange translateSourceRange(const SourceManager& SM, const LangOptions& LangOpts, const CharSourceRange& R) {
        // We want the last character in this location, so we will adjust the
        // location accordingly.

        SourceLocation EndLoc = R.getEnd();
        bool IsTokenRange = R.isTokenRange();

        if (EndLoc.isValid() && EndLoc.isMacroID() && !SM.isMacroArgExpansion(EndLoc)) {
            CharSourceRange Expansion = SM.getExpansionRange(EndLoc);
            EndLoc = Expansion.getEnd();
            IsTokenRange = Expansion.isTokenRange();
        }

        if (IsTokenRange && EndLoc.isValid()) {
            unsigned Length = Lexer::MeasureTokenLength(SM.getSpellingLoc(EndLoc), SM, LangOpts);
            EndLoc = EndLoc.getLocWithOffset(Length);
        }

        CXSourceRange Result = {
            {
                &SM,
                &LangOpts
            },
            R.getBegin().getRawEncoding(),
            EndLoc.getRawEncoding()
        };
        return Result;
    }

    CXSourceRange translateSourceRangeRaw(const SourceManager& SM, const LangOptions& LangOpts, const CharSourceRange& R) {
        CXSourceRange Result = {
            {
                &SM,
                &LangOpts
            },
            R.getBegin().getRawEncoding(),
            R.getEnd().getRawEncoding()
        };
        return Result;
    }
}
