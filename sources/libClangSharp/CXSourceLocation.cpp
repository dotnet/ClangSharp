// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-10.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "ClangSharp.h"
#include "CXCursor.h"
#include "CXSourceLocation.h"

#include <clang/Basic/SourceLocation.h>
#include <clang/Frontend/ASTUnit.h>
#include <clang/Lex/Lexer.h>

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

clang::SourceRange getRawCursorExtent(CXCursor C) {
    using namespace clang;
    using namespace clang::cxcursor;

    if (clang_isReference(C.kind)) {
        switch (C.kind) {
            case CXCursor_ObjCSuperClassRef:
                return getCursorObjCSuperClassRef(C).second;

            case CXCursor_ObjCProtocolRef:
                return getCursorObjCProtocolRef(C).second;

            case CXCursor_ObjCClassRef:
                return getCursorObjCClassRef(C).second;

            case CXCursor_TypeRef:
                return getCursorTypeRef(C).second;

            case CXCursor_TemplateRef:
                return getCursorTemplateRef(C).second;

            case CXCursor_NamespaceRef:
                return getCursorNamespaceRef(C).second;

            case CXCursor_MemberRef:
                return getCursorMemberRef(C).second;

            case CXCursor_CXXBaseSpecifier:
                return getCursorCXXBaseSpecifier(C)->getSourceRange();

            case CXCursor_LabelRef:
                return getCursorLabelRef(C).second;

            case CXCursor_OverloadedDeclRef:
                return getCursorOverloadedDeclRef(C).second;

            case CXCursor_VariableRef:
                return getCursorVariableRef(C).second;

            default:
                // FIXME: Need a way to enumerate all non-reference cases.
                llvm_unreachable("Missed a reference kind");
        }
    }

    if (clang_isExpression(C.kind))
        return getCursorExpr(C)->getSourceRange();

    if (clang_isStatement(C.kind))
        return getCursorStmt(C)->getSourceRange();

    if (clang_isAttribute(C.kind))
        return getCursorAttr(C)->getRange();

    if (C.kind == CXCursor_PreprocessingDirective)
        return getCursorPreprocessingDirective(C);

    if (C.kind == CXCursor_MacroExpansion) {
        ASTUnit* TU = getCursorASTUnit(C);
        SourceRange Range = getCursorMacroExpansion(C).getSourceRange();
        return TU->mapRangeFromPreamble(Range);
    }

    if (C.kind == CXCursor_MacroDefinition) {
        ASTUnit* TU = getCursorASTUnit(C);
        SourceRange Range = getCursorMacroDefinition(C)->getSourceRange();
        return TU->mapRangeFromPreamble(Range);
    }

    if (C.kind == CXCursor_InclusionDirective) {
        ASTUnit* TU = getCursorASTUnit(C);
        SourceRange Range = getCursorInclusionDirective(C)->getSourceRange();
        return TU->mapRangeFromPreamble(Range);
    }

    if (C.kind == CXCursor_TranslationUnit) {
        ASTUnit* TU = getCursorASTUnit(C);
        FileID MainID = TU->getSourceManager().getMainFileID();
        SourceLocation Start = TU->getSourceManager().getLocForStartOfFile(MainID);
        SourceLocation End = TU->getSourceManager().getLocForEndOfFile(MainID);
        return SourceRange(Start, End);
    }

    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        if (!D)
            return SourceRange();

        SourceRange R = D->getSourceRange();
        // FIXME: Multiple variables declared in a single declaration
        // currently lack the information needed to correctly determine their
        // ranges when accounting for the type-specifier. We use context
        // stored in the CXCursor to determine if the VarDecl is in a DeclGroup,
        // and if so, whether it is the first decl.
        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            if (!isFirstInDeclGroup(C))
                R.setBegin(VD->getLocation());
        }
        return R;
    }
    return SourceRange();
}

bool isASTUnitSourceLocation(const CXSourceLocation& L) {
    // If the lowest bit is clear then the first ptr_data entry is a SourceManager
    // pointer, or the CXSourceLocation is a null location.
    return ((uintptr_t)L.ptr_data[0] & 0x1) == 0;
}

namespace clang::cxloc {
    CXSourceRange translateSourceRange(ASTContext& Context, SourceRange R) {
        return translateSourceRange(Context.getSourceManager(), Context.getLangOpts(), CharSourceRange::getTokenRange(R));
    }

    CXSourceRange translateSourceRange(const SourceManager& SM, const LangOptions& LangOpts, const CharSourceRange& R) {
        // We want the last character in this location, so we will adjust the
        // location accordingly.
        SourceLocation EndLoc = R.getEnd();
        bool IsTokenRange = R.isTokenRange();
        if (IsTokenRange && EndLoc.isValid()) {
            unsigned Length = Lexer::MeasureTokenLength(SM.getSpellingLoc(EndLoc), SM, LangOpts);
            EndLoc = EndLoc.getLocWithOffset(Length);
        }

        CXSourceRange Result = {
            {& SM,& LangOpts },
            R.getBegin().getRawEncoding(),
            EndLoc.getRawEncoding()
        };
        return Result;
    }
}
