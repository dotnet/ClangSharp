// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-10.0.0/clang/tools/libclang
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CXCURSOR_H
#define LIBCLANGSHARP_CXCURSOR_H

#include <clang/AST/Attr.h>
#include <clang/AST/Decl.h>
#include <clang/AST/Expr.h>
#include <clang/AST/Stmt.h>
#include <clang/AST/TemplateName.h>
#include <clang/Frontend/ASTUnit.h>
#include <clang-c/Index.h>

#include <llvm/ADT/PointerUnion.h>

namespace clang::cxcursor {
    typedef llvm::PointerUnion<const OverloadExpr*, const Decl*, OverloadedTemplateStorage* > OverloadedDeclRefStorage;

    /// Wraps a macro expansion cursor and provides a common interface
    /// for a normal macro expansion cursor or a "pseudo" one.
    ///
    /// "Pseudo" macro expansion cursors (essentially a macro definition along with
    /// a source location) are created in special cases, for example they can be
    /// created for identifiers inside macro definitions, if these identifiers are
    /// macro names.
    class MacroExpansionCursor {
        CXCursor C;

        bool isPseudo() const { return C.data[1] != nullptr; }
        const MacroDefinitionRecord* getAsMacroDefinition() const {
            assert(isPseudo());
            return static_cast<const MacroDefinitionRecord*>(C.data[0]);
        }
        const MacroExpansion* getAsMacroExpansion() const {
            assert(!isPseudo());
            return static_cast<const MacroExpansion*>(C.data[0]);
        }
        SourceLocation getPseudoLoc() const {
            assert(isPseudo());
            return SourceLocation::getFromPtrEncoding(C.data[1]);
        }

    public:
        MacroExpansionCursor(CXCursor C) : C(C) {
            assert(C.kind == CXCursor_MacroExpansion);
        }

        const IdentifierInfo* getName() const;
        const MacroDefinitionRecord* getDefinition() const;
        SourceRange getSourceRange() const;
    };

    ASTUnit* getCursorASTUnit(CXCursor Cursor);
    ASTContext& getCursorContext(CXCursor Cursor);
    CXTranslationUnit getCursorTU(CXCursor Cursor);

    const Attr* getCursorAttr(CXCursor Cursor);
    const Decl* getCursorDecl(CXCursor Cursor);
    const Expr* getCursorExpr(CXCursor Cursor);
    const Stmt* getCursorStmt(CXCursor Cursor);

    /// Unpack a CXXBaseSpecifier cursor into a CXXBaseSpecifier.
    const CXXBaseSpecifier* getCursorCXXBaseSpecifier(CXCursor C);

    /// Unpack a given inclusion directive cursor to retrieve its source range.
    const InclusionDirective* getCursorInclusionDirective(CXCursor C);

    /// Unpack a given macro definition cursor to retrieve its source range.
    const MacroDefinitionRecord* getCursorMacroDefinition(CXCursor C);

    /// Unpack a given macro expansion cursor to retrieve its info.
    MacroExpansionCursor getCursorMacroExpansion(CXCursor C);

    /// Unpack a given preprocessing directive to retrieve its source range.
    SourceRange getCursorPreprocessingDirective(CXCursor C);

    /// Unpack a label reference into the label statement it refers to and the location of the reference.
    std::pair<const LabelStmt*, SourceLocation> getCursorLabelRef(CXCursor C);

    /// Unpack a MemberRef cursor into the field it references and the location where the reference occurred.
    std::pair<const FieldDecl*, SourceLocation> getCursorMemberRef(CXCursor C);

    /// Unpack a NamespaceRef cursor into the namespace or namespace alias it references and the location where the reference occurred.
    std::pair<const NamedDecl*, SourceLocation> getCursorNamespaceRef(CXCursor C);

    /// Unpack an ObjCClassRef cursor into the class it references and optionally the location where the reference occurred.
    std::pair<const ObjCInterfaceDecl*, SourceLocation> getCursorObjCClassRef(CXCursor C);

    /// Unpack an ObjCProtocolRef cursor into the protocol it references and optionally the location where the reference occurred.
    std::pair<const ObjCProtocolDecl*, SourceLocation> getCursorObjCProtocolRef(CXCursor C);

    /// Unpack an ObjCSuperClassRef cursor into the interface it references and optionally the location where the reference occurred.
    std::pair<const ObjCInterfaceDecl*, SourceLocation> getCursorObjCSuperClassRef(CXCursor C);

    /// Unpack an overloaded declaration reference into an expression, declaration, or template name along with the source location.
    std::pair<OverloadedDeclRefStorage, SourceLocation> getCursorOverloadedDeclRef(CXCursor C);

    /// Unpack a TemplateRef cursor into the template it references and the location where the reference occurred.
    std::pair<const TemplateDecl*, SourceLocation> getCursorTemplateRef(CXCursor C);

    /// Unpack a TypeRef cursor into the class it references and optionally the location where the reference occurred.
    std::pair<const TypeDecl*, SourceLocation> getCursorTypeRef(CXCursor C);

    /// Unpack a VariableRef cursor into the variable it references and the location where the where the reference occurred.
    std::pair<const VarDecl*, SourceLocation> getCursorVariableRef(CXCursor C);

    // FIXME: Remove once we can model DeclGroups and their appropriate ranges properly in the ASTs.
    bool isFirstInDeclGroup(CXCursor C);
}

#endif
