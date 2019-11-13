// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-9.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#include "ClangSharp.h"
#include "CXCursor.h"
#include "CXTranslationUnit.h"

using namespace clang::cxtu;

namespace clang::cxcursor {
    const IdentifierInfo* MacroExpansionCursor::getName() const {
        if (isPseudo())
            return getAsMacroDefinition()->getName();
        return getAsMacroExpansion()->getName();
    }

    const MacroDefinitionRecord* MacroExpansionCursor::getDefinition() const {
        if (isPseudo())
            return getAsMacroDefinition();
        return getAsMacroExpansion()->getDefinition();
    }

    SourceRange MacroExpansionCursor::getSourceRange() const {
        if (isPseudo())
            return getPseudoLoc();
        return getAsMacroExpansion()->getSourceRange();
    }

    ASTUnit* getCursorASTUnit(CXCursor Cursor) {
        CXTranslationUnit TU = getCursorTU(Cursor);
        if (!TU)
            return nullptr;
        return getASTUnit(TU);
    }

    ASTContext& getCursorContext(CXCursor Cursor) {
        return getCursorASTUnit(Cursor)->getASTContext();
    }

    CXTranslationUnit getCursorTU(CXCursor Cursor) {
        return static_cast<CXTranslationUnit>(const_cast<void*>(Cursor.data[2]));
    }

    const Attr* getCursorAttr(CXCursor Cursor) {
        return static_cast<const Attr*>(Cursor.data[1]);
    }

    const Decl* getCursorDecl(CXCursor Cursor) {
        return static_cast<const Decl*>(Cursor.data[0]);
    }

    const Expr* getCursorExpr(CXCursor Cursor) {
        return dyn_cast_or_null<Expr>(getCursorStmt(Cursor));
    }

    const Stmt* getCursorStmt(CXCursor Cursor) {
        if (Cursor.kind == CXCursor_ObjCSuperClassRef ||
            Cursor.kind == CXCursor_ObjCProtocolRef ||
            Cursor.kind == CXCursor_ObjCClassRef)
            return nullptr;

        return static_cast<const Stmt*>(Cursor.data[1]);
    }

    const CXXBaseSpecifier* getCursorCXXBaseSpecifier(CXCursor C) {
        assert(C.kind == CXCursor_CXXBaseSpecifier);
        return static_cast<const CXXBaseSpecifier*>(C.data[0]);
    }

    const InclusionDirective* getCursorInclusionDirective(CXCursor C) {
        assert(C.kind == CXCursor_InclusionDirective);
        return static_cast<const InclusionDirective*>(C.data[0]);
    }

    const MacroDefinitionRecord* getCursorMacroDefinition(CXCursor C) {
        assert(C.kind == CXCursor_MacroDefinition);
        return static_cast<const MacroDefinitionRecord*>(C.data[0]);
    }

    MacroExpansionCursor getCursorMacroExpansion(CXCursor C) {
        return C;
    }

    SourceRange getCursorPreprocessingDirective(CXCursor C) {
        assert(C.kind == CXCursor_PreprocessingDirective);
        SourceRange Range(SourceLocation::getFromPtrEncoding(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
        ASTUnit* TU = getCursorASTUnit(C);
        return TU->mapRangeFromPreamble(Range);
    }

    std::pair<const LabelStmt*, SourceLocation> getCursorLabelRef(CXCursor C) {
        assert(C.kind == CXCursor_LabelRef);
        return std::make_pair(static_cast<const LabelStmt*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<const FieldDecl*, SourceLocation> getCursorMemberRef(CXCursor C) {
        assert(C.kind == CXCursor_MemberRef);
        return std::make_pair(static_cast<const FieldDecl*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<const NamedDecl*, SourceLocation> getCursorNamespaceRef(CXCursor C) {
        assert(C.kind == CXCursor_NamespaceRef);
        return std::make_pair(static_cast<const NamedDecl*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<const ObjCInterfaceDecl*, SourceLocation> getCursorObjCClassRef(CXCursor C) {
        assert(C.kind == CXCursor_ObjCClassRef);
        return std::make_pair(static_cast<const ObjCInterfaceDecl*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<const ObjCProtocolDecl*, SourceLocation> getCursorObjCProtocolRef(CXCursor C) {
        assert(C.kind == CXCursor_ObjCProtocolRef);
        return std::make_pair(static_cast<const ObjCProtocolDecl*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<const ObjCInterfaceDecl*, SourceLocation> getCursorObjCSuperClassRef(CXCursor C) {
        assert(C.kind == CXCursor_ObjCSuperClassRef);
        return std::make_pair(static_cast<const ObjCInterfaceDecl*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<OverloadedDeclRefStorage, SourceLocation> getCursorOverloadedDeclRef(CXCursor C) {
        assert(C.kind == CXCursor_OverloadedDeclRef);
        return std::make_pair(OverloadedDeclRefStorage::getFromOpaqueValue(const_cast<void*>(C.data[0])), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<const TemplateDecl*, SourceLocation> getCursorTemplateRef(CXCursor C) {
        assert(C.kind == CXCursor_TemplateRef);
        return std::make_pair(static_cast<const TemplateDecl*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<const TypeDecl*, SourceLocation> getCursorTypeRef(CXCursor C) {
        assert(C.kind == CXCursor_TypeRef);
        return std::make_pair(static_cast<const TypeDecl*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    std::pair<const VarDecl*, SourceLocation> getCursorVariableRef(CXCursor C) {
        assert(C.kind == CXCursor_VariableRef);
        return std::make_pair(static_cast<const VarDecl*>(C.data[0]), SourceLocation::getFromPtrEncoding(C.data[1]));
    }

    bool isFirstInDeclGroup(CXCursor C) {
        assert(clangsharp_isDeclaration(C.kind));
        return ((uintptr_t)(C.data[1])) != 0;
    }
}
