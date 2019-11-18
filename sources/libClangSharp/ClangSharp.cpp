// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

#include "ClangSharp.h"
#include "CXCursor.h"
#include "CXLoadedDiagnostic.h"
#include "CXSourceLocation.h"
#include "CXString.h"
#include "CXType.h"

#include <clang/Basic/SourceManager.h>

using namespace clang;
using namespace clang::cxcursor;
using namespace clang::cxloc;
using namespace clang::cxstring;

CX_AttrKind clangsharp_Cursor_getAttrKind(CXCursor C) {
    if (clangsharp_isAttribute(C.kind)) {
        const Attr* A = getCursorAttr(C);
        return static_cast<CX_AttrKind>(A->getKind() + 1);
    }

    return CX_AttrKind_Invalid;
}

CX_BinaryOperatorKind clangsharp_Cursor_getBinaryOpcode(CXCursor C) {
    if (C.kind == CXCursor_BinaryOperator || C.kind == CXCursor_CompoundAssignOperator) {
        const Expr* E = getCursorExpr(C);
        if (const BinaryOperator* BinOp = dyn_cast<BinaryOperator>(E)) {
            return static_cast<CX_BinaryOperatorKind>(BinOp->getOpcode() + 1);
        }
    }

    return CX_BO_Invalid;
}

CXString clangsharp_Cursor_getBinaryOpcodeSpelling(CX_BinaryOperatorKind Op) {
    if (Op != CX_BO_Invalid) {
        return createDup(
            BinaryOperator::getOpcodeStr(static_cast<BinaryOperatorKind>(Op - 1)));
    }

    return createEmpty();
}

CX_DeclKind clangsharp_Cursor_getDeclKind(CXCursor C) {
    if (clangsharp_isDeclaration(C.kind) || clangsharp_isTranslationUnit(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return static_cast<CX_DeclKind>(D->getKind() + 1);
    }

    return CX_DeclKind_Invalid;
}

CX_StmtClass clangsharp_Cursor_getStmtClass(CXCursor C) {
    if (clangsharp_isExpression(C.kind) || clangsharp_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);
        return static_cast<CX_StmtClass>(S->getStmtClass());
    }

    return CX_StmtClass_Invalid;
}

CX_UnaryOperatorKind clangsharp_Cursor_getUnaryOpcode(CXCursor C) {
    if (C.kind == CXCursor_UnaryOperator) {
        const Expr* E = getCursorExpr(C);
        if (const UnaryOperator* UnOp = dyn_cast<UnaryOperator>(E)) {
            return static_cast<CX_UnaryOperatorKind>(UnOp->getOpcode() + 1);
        }
    }

    return CX_UO_Invalid;
}

CXString clangsharp_Cursor_getUnaryOpcodeSpelling(CX_UnaryOperatorKind Op) {
    if (Op != CX_UO_Invalid) {
        return createDup(
            UnaryOperator::getOpcodeStr(static_cast<UnaryOperatorKind>(Op - 1)));
    }

    return createEmpty();
}

CXSourceRange clangsharp_getCursorExtent(CXCursor C) {
    SourceRange R = getRawCursorExtent(C);
    if (R.isInvalid())
        return clangsharp_getNullRange();

    return translateSourceRange(getCursorContext(C), R);
}

CXSourceRange clangsharp_getNullRange() {
    CXSourceRange Result = { { nullptr, nullptr }, 0, 0 };
    return Result;
}

CXSourceRange clangsharp_getRange(CXSourceLocation begin, CXSourceLocation end) {
    if (!isASTUnitSourceLocation(begin)) {
        if (isASTUnitSourceLocation(end))
            return clangsharp_getNullRange();
        CXSourceRange Result = { { begin.ptr_data[0], end.ptr_data[0] }, 0, 0 };
        return Result;
    }

    if (begin.ptr_data[0] != end.ptr_data[0] || begin.ptr_data[1] != end.ptr_data[1])
        return clangsharp_getNullRange();

    CXSourceRange Result = {
        { begin.ptr_data[0], begin.ptr_data[1] },
        begin.int_data, end.int_data
    };
    return Result;
}

void clangsharp_getSpellingLocation(CXSourceLocation location, CXFile* file, unsigned* line, unsigned* column, unsigned* offset) {
    if (!isASTUnitSourceLocation(location)) {
        CXLoadedDiagnostic::decodeLocation(location, file, line, column, offset);
        return;
    }

    SourceLocation Loc = SourceLocation::getFromRawEncoding(location.int_data);

    if (!location.ptr_data[0] || Loc.isInvalid())
        return createNullLocation(file, line, column, offset);

    const SourceManager& SM = *static_cast<const SourceManager*>(location.ptr_data[0]);
    SourceLocation SpellLoc = SM.getSpellingLoc(Loc);
    std::pair<FileID, unsigned> LocInfo = SM.getDecomposedLoc(SpellLoc);
    FileID FID = LocInfo.first;
    unsigned FileOffset = LocInfo.second;

    if (FID.isInvalid())
        return createNullLocation(file, line, column, offset);

    if (file)
        *file = const_cast<FileEntry*>(SM.getFileEntryForID(FID));
    if (line)
        *line = SM.getLineNumber(FID, FileOffset);
    if (column)
        *column = SM.getColumnNumber(FID, FileOffset);
    if (offset)
        *offset = FileOffset;
}

unsigned clangsharp_isAttribute(CXCursorKind K) {
    return K >= CXCursor_FirstAttr && K <= CXCursor_LastAttr;
}

unsigned clangsharp_isDeclaration(CXCursorKind K) {
    return (K >= CXCursor_FirstDecl && K <= CXCursor_LastDecl) ||
           (K >= CXCursor_FirstExtraDecl && K <= CXCursor_LastExtraDecl);
}

unsigned clangsharp_isExpression(CXCursorKind K) {
    return K >= CXCursor_FirstExpr && K <= CXCursor_LastExpr;
}

unsigned clangsharp_isReference(CXCursorKind K) {
    return K >= CXCursor_FirstRef && K <= CXCursor_LastRef;
}

unsigned clangsharp_isStatement(CXCursorKind K) {
    return K >= CXCursor_FirstStmt && K <= CXCursor_LastStmt;
}

unsigned clangsharp_isTranslationUnit(CXCursorKind K) {
    return K == CXCursor_TranslationUnit;
}

CX_TypeClass clangsharp_Type_getTypeClass(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (TP != nullptr)
    {
        return static_cast<CX_TypeClass>(TP->getTypeClass() + 1);
    }

    return CX_TypeClass_Invalid;
}
