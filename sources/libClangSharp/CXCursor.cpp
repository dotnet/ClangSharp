// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-13.0.0/clang/tools/libclang
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

    const PreprocessedEntity* getCursorPreprocessedEntity(CXCursor Cursor) {
        return static_cast<const PreprocessedEntity*>(Cursor.data[0]);
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
        assert(clang_isDeclaration(C.kind));
        return ((uintptr_t)(C.data[1])) != 0;
    }

    static CXCursorKind GetCursorKind(const Attr* A) {
        assert(A && "Invalid arguments!");
        switch (A->getKind()) {
        default: break;
        case attr::IBAction: return CXCursor_IBActionAttr;
        case attr::IBOutlet: return CXCursor_IBOutletAttr;
        case attr::IBOutletCollection: return CXCursor_IBOutletCollectionAttr;
        case attr::Final: return CXCursor_CXXFinalAttr;
        case attr::Override: return CXCursor_CXXOverrideAttr;
        case attr::Annotate: return CXCursor_AnnotateAttr;
        case attr::AsmLabel: return CXCursor_AsmLabelAttr;
        case attr::Packed: return CXCursor_PackedAttr;
        case attr::Pure: return CXCursor_PureAttr;
        case attr::Const: return CXCursor_ConstAttr;
        case attr::NoDuplicate: return CXCursor_NoDuplicateAttr;
        case attr::CUDAConstant: return CXCursor_CUDAConstantAttr;
        case attr::CUDADevice: return CXCursor_CUDADeviceAttr;
        case attr::CUDAGlobal: return CXCursor_CUDAGlobalAttr;
        case attr::CUDAHost: return CXCursor_CUDAHostAttr;
        case attr::CUDAShared: return CXCursor_CUDASharedAttr;
        case attr::Visibility: return CXCursor_VisibilityAttr;
        case attr::DLLExport: return CXCursor_DLLExport;
        case attr::DLLImport: return CXCursor_DLLImport;
        case attr::NSReturnsRetained: return CXCursor_NSReturnsRetained;
        case attr::NSReturnsNotRetained: return CXCursor_NSReturnsNotRetained;
        case attr::NSReturnsAutoreleased: return CXCursor_NSReturnsAutoreleased;
        case attr::NSConsumesSelf: return CXCursor_NSConsumesSelf;
        case attr::NSConsumed: return CXCursor_NSConsumed;
        case attr::ObjCException: return CXCursor_ObjCException;
        case attr::ObjCNSObject: return CXCursor_ObjCNSObject;
        case attr::ObjCIndependentClass: return CXCursor_ObjCIndependentClass;
        case attr::ObjCPreciseLifetime: return CXCursor_ObjCPreciseLifetime;
        case attr::ObjCReturnsInnerPointer: return CXCursor_ObjCReturnsInnerPointer;
        case attr::ObjCRequiresSuper: return CXCursor_ObjCRequiresSuper;
        case attr::ObjCRootClass: return CXCursor_ObjCRootClass;
        case attr::ObjCSubclassingRestricted: return CXCursor_ObjCSubclassingRestricted;
        case attr::ObjCExplicitProtocolImpl: return CXCursor_ObjCExplicitProtocolImpl;
        case attr::ObjCDesignatedInitializer: return CXCursor_ObjCDesignatedInitializer;
        case attr::ObjCRuntimeVisible: return CXCursor_ObjCRuntimeVisible;
        case attr::ObjCBoxable: return CXCursor_ObjCBoxable;
        case attr::FlagEnum: return CXCursor_FlagEnum;
        case attr::Convergent: return CXCursor_ConvergentAttr;
        case attr::WarnUnused: return CXCursor_WarnUnusedAttr;
        case attr::WarnUnusedResult: return CXCursor_WarnUnusedResultAttr;
        case attr::Aligned: return CXCursor_AlignedAttr;
        }

        return CXCursor_UnexposedAttr;
    }

    CXCursor MakeCXCursor(const Attr* A, const clang::Decl* Parent, CXTranslationUnit TU) {
        if (!A) {
            return clang_getNullCursor();
        }

        assert(A && Parent && TU && "Invalid arguments!");
        CXCursor C = { GetCursorKind(A), 0, { Parent, A, TU } };
        return C;
    }

    CXCursor MakeCXCursor(const Decl* D, CXTranslationUnit TU, SourceRange RegionOfInterest, bool FirstInDeclGroup) {
        if (!D) {
            return clang_getNullCursor();
        }

        assert(D && TU && "Invalid arguments!");

        CXCursorKind K = getCursorKindForDecl(D);

        if (K == CXCursor_ObjCClassMethodDecl ||
            K == CXCursor_ObjCInstanceMethodDecl) {
            int SelectorIdIndex = -1;
            // Check if cursor points to a selector id.
            if (RegionOfInterest.isValid() &&
                RegionOfInterest.getBegin() == RegionOfInterest.getEnd()) {
                SmallVector<SourceLocation, 16> SelLocs;
                cast<ObjCMethodDecl>(D)->getSelectorLocs(SelLocs);
                SmallVectorImpl<SourceLocation>::iterator I =
                    llvm::find(SelLocs, RegionOfInterest.getBegin());
                if (I != SelLocs.end())
                    SelectorIdIndex = I - SelLocs.begin();
            }
            CXCursor C = { K, SelectorIdIndex,
                           { D, (void*)(intptr_t)(FirstInDeclGroup ? 1 : 0), TU } };
            return C;
        }

        CXCursor C = { K, 0, { D, (void*)(intptr_t)(FirstInDeclGroup ? 1 : 0), TU } };
        return C;
    }

    static CXCursor getSelectorIdentifierCursor(int SelIdx, CXCursor cursor) {
        CXCursor newCursor = cursor;

        if (cursor.kind == CXCursor_ObjCMessageExpr) {
            if (SelIdx == -1 ||
                unsigned(SelIdx) >= cast<ObjCMessageExpr>(getCursorExpr(cursor))
                ->getNumSelectorLocs())
                newCursor.xdata = -1;
            else
                newCursor.xdata = SelIdx;
        }
        else if (cursor.kind == CXCursor_ObjCClassMethodDecl ||
            cursor.kind == CXCursor_ObjCInstanceMethodDecl) {
            if (SelIdx == -1 ||
                unsigned(SelIdx) >= cast<ObjCMethodDecl>(getCursorDecl(cursor))
                ->getNumSelectorLocs())
                newCursor.xdata = -1;
            else
                newCursor.xdata = SelIdx;
        }

        return newCursor;
    }

    CXCursor MakeCXCursor(const Stmt* S, const Decl* Parent, CXTranslationUnit TU, SourceRange RegionOfInterest) {
        if (!S) {
            return clang_getNullCursor();
        }

        assert(S && TU && "Invalid arguments!");
        CXCursorKind K = CXCursor_NotImplemented;

        switch (S->getStmtClass()) {
        case Stmt::NoStmtClass:
            break;

        case Stmt::CaseStmtClass:
            K = CXCursor_CaseStmt;
            break;

        case Stmt::DefaultStmtClass:
            K = CXCursor_DefaultStmt;
            break;

        case Stmt::IfStmtClass:
            K = CXCursor_IfStmt;
            break;

        case Stmt::SwitchStmtClass:
            K = CXCursor_SwitchStmt;
            break;

        case Stmt::WhileStmtClass:
            K = CXCursor_WhileStmt;
            break;

        case Stmt::DoStmtClass:
            K = CXCursor_DoStmt;
            break;

        case Stmt::ForStmtClass:
            K = CXCursor_ForStmt;
            break;

        case Stmt::GotoStmtClass:
            K = CXCursor_GotoStmt;
            break;

        case Stmt::IndirectGotoStmtClass:
            K = CXCursor_IndirectGotoStmt;
            break;

        case Stmt::ContinueStmtClass:
            K = CXCursor_ContinueStmt;
            break;

        case Stmt::BreakStmtClass:
            K = CXCursor_BreakStmt;
            break;

        case Stmt::ReturnStmtClass:
            K = CXCursor_ReturnStmt;
            break;

        case Stmt::GCCAsmStmtClass:
            K = CXCursor_GCCAsmStmt;
            break;

        case Stmt::MSAsmStmtClass:
            K = CXCursor_MSAsmStmt;
            break;

        case Stmt::ObjCAtTryStmtClass:
            K = CXCursor_ObjCAtTryStmt;
            break;

        case Stmt::ObjCAtCatchStmtClass:
            K = CXCursor_ObjCAtCatchStmt;
            break;

        case Stmt::ObjCAtFinallyStmtClass:
            K = CXCursor_ObjCAtFinallyStmt;
            break;

        case Stmt::ObjCAtThrowStmtClass:
            K = CXCursor_ObjCAtThrowStmt;
            break;

        case Stmt::ObjCAtSynchronizedStmtClass:
            K = CXCursor_ObjCAtSynchronizedStmt;
            break;

        case Stmt::ObjCAutoreleasePoolStmtClass:
            K = CXCursor_ObjCAutoreleasePoolStmt;
            break;

        case Stmt::ObjCForCollectionStmtClass:
            K = CXCursor_ObjCForCollectionStmt;
            break;

        case Stmt::CXXCatchStmtClass:
            K = CXCursor_CXXCatchStmt;
            break;

        case Stmt::CXXTryStmtClass:
            K = CXCursor_CXXTryStmt;
            break;

        case Stmt::CXXForRangeStmtClass:
            K = CXCursor_CXXForRangeStmt;
            break;

        case Stmt::SEHTryStmtClass:
            K = CXCursor_SEHTryStmt;
            break;

        case Stmt::SEHExceptStmtClass:
            K = CXCursor_SEHExceptStmt;
            break;

        case Stmt::SEHFinallyStmtClass:
            K = CXCursor_SEHFinallyStmt;
            break;

        case Stmt::SEHLeaveStmtClass:
            K = CXCursor_SEHLeaveStmt;
            break;

        case Stmt::CoroutineBodyStmtClass:
        case Stmt::CoreturnStmtClass:
            K = CXCursor_UnexposedStmt;
            break;

        case Stmt::ArrayTypeTraitExprClass:
        case Stmt::AsTypeExprClass:
        case Stmt::AtomicExprClass:
        case Stmt::BinaryConditionalOperatorClass:
        case Stmt::TypeTraitExprClass:
        case Stmt::CoawaitExprClass:
        case Stmt::ConceptSpecializationExprClass:
        case Stmt::RequiresExprClass:
        case Stmt::DependentCoawaitExprClass:
        case Stmt::CoyieldExprClass:
        case Stmt::CXXBindTemporaryExprClass:
        case Stmt::CXXDefaultArgExprClass:
        case Stmt::CXXDefaultInitExprClass:
        case Stmt::CXXFoldExprClass:
        case Stmt::CXXRewrittenBinaryOperatorClass:
        case Stmt::CXXStdInitializerListExprClass:
        case Stmt::CXXScalarValueInitExprClass:
        case Stmt::CXXUuidofExprClass:
        case Stmt::ChooseExprClass:
        case Stmt::DesignatedInitExprClass:
        case Stmt::DesignatedInitUpdateExprClass:
        case Stmt::ArrayInitLoopExprClass:
        case Stmt::ArrayInitIndexExprClass:
        case Stmt::ExprWithCleanupsClass:
        case Stmt::ExpressionTraitExprClass:
        case Stmt::ExtVectorElementExprClass:
        case Stmt::ImplicitCastExprClass:
        case Stmt::ImplicitValueInitExprClass:
        case Stmt::NoInitExprClass:
        case Stmt::MaterializeTemporaryExprClass:
        case Stmt::ObjCIndirectCopyRestoreExprClass:
        case Stmt::OffsetOfExprClass:
        case Stmt::ParenListExprClass:
        case Stmt::PredefinedExprClass:
        case Stmt::ShuffleVectorExprClass:
        case Stmt::SourceLocExprClass:
        case Stmt::ConvertVectorExprClass:
        case Stmt::VAArgExprClass:
        case Stmt::ObjCArrayLiteralClass:
        case Stmt::ObjCDictionaryLiteralClass:
        case Stmt::ObjCBoxedExprClass:
        case Stmt::ObjCSubscriptRefExprClass:
        case Stmt::RecoveryExprClass:
            K = CXCursor_UnexposedExpr;
            break;

        case Stmt::OpaqueValueExprClass:
            if (Expr* Src = cast<OpaqueValueExpr>(S)->getSourceExpr())
                return MakeCXCursor(Src, Parent, TU, RegionOfInterest);
            K = CXCursor_UnexposedExpr;
            break;

        case Stmt::PseudoObjectExprClass:
            return MakeCXCursor(cast<PseudoObjectExpr>(S)->getSyntacticForm(),
                Parent, TU, RegionOfInterest);

        case Stmt::CompoundStmtClass:
            K = CXCursor_CompoundStmt;
            break;

        case Stmt::NullStmtClass:
            K = CXCursor_NullStmt;
            break;

        case Stmt::LabelStmtClass:
            K = CXCursor_LabelStmt;
            break;

        case Stmt::AttributedStmtClass:
            K = CXCursor_UnexposedStmt;
            break;

        case Stmt::DeclStmtClass:
            K = CXCursor_DeclStmt;
            break;

        case Stmt::CapturedStmtClass:
            K = CXCursor_UnexposedStmt;
            break;

        case Stmt::IntegerLiteralClass:
            K = CXCursor_IntegerLiteral;
            break;

        case Stmt::FixedPointLiteralClass:
            K = CXCursor_FixedPointLiteral;
            break;

        case Stmt::FloatingLiteralClass:
            K = CXCursor_FloatingLiteral;
            break;

        case Stmt::ImaginaryLiteralClass:
            K = CXCursor_ImaginaryLiteral;
            break;

        case Stmt::StringLiteralClass:
            K = CXCursor_StringLiteral;
            break;

        case Stmt::CharacterLiteralClass:
            K = CXCursor_CharacterLiteral;
            break;

        case Stmt::ConstantExprClass:
            return MakeCXCursor(cast<ConstantExpr>(S)->getSubExpr(),
                Parent, TU, RegionOfInterest);

        case Stmt::ParenExprClass:
            K = CXCursor_ParenExpr;
            break;

        case Stmt::UnaryOperatorClass:
            K = CXCursor_UnaryOperator;
            break;

        case Stmt::UnaryExprOrTypeTraitExprClass:
        case Stmt::CXXNoexceptExprClass:
            K = CXCursor_UnaryExpr;
            break;

        case Stmt::MSPropertySubscriptExprClass:
        case Stmt::ArraySubscriptExprClass:
            K = CXCursor_ArraySubscriptExpr;
            break;

        case Stmt::MatrixSubscriptExprClass:
            // TODO: add support for MatrixSubscriptExpr.
            K = CXCursor_UnexposedExpr;
            break;

        case Stmt::OMPArraySectionExprClass:
            K = CXCursor_OMPArraySectionExpr;
            break;

        case Stmt::OMPArrayShapingExprClass:
            K = CXCursor_OMPArrayShapingExpr;
            break;

        case Stmt::OMPIteratorExprClass:
            K = CXCursor_OMPIteratorExpr;
            break;

        case Stmt::BinaryOperatorClass:
            K = CXCursor_BinaryOperator;
            break;

        case Stmt::CompoundAssignOperatorClass:
            K = CXCursor_CompoundAssignOperator;
            break;

        case Stmt::ConditionalOperatorClass:
            K = CXCursor_ConditionalOperator;
            break;

        case Stmt::CStyleCastExprClass:
            K = CXCursor_CStyleCastExpr;
            break;

        case Stmt::CompoundLiteralExprClass:
            K = CXCursor_CompoundLiteralExpr;
            break;

        case Stmt::InitListExprClass:
            K = CXCursor_InitListExpr;
            break;

        case Stmt::AddrLabelExprClass:
            K = CXCursor_AddrLabelExpr;
            break;

        case Stmt::StmtExprClass:
            K = CXCursor_StmtExpr;
            break;

        case Stmt::GenericSelectionExprClass:
            K = CXCursor_GenericSelectionExpr;
            break;

        case Stmt::GNUNullExprClass:
            K = CXCursor_GNUNullExpr;
            break;

        case Stmt::CXXStaticCastExprClass:
            K = CXCursor_CXXStaticCastExpr;
            break;

        case Stmt::CXXDynamicCastExprClass:
            K = CXCursor_CXXDynamicCastExpr;
            break;

        case Stmt::CXXReinterpretCastExprClass:
            K = CXCursor_CXXReinterpretCastExpr;
            break;

        case Stmt::CXXConstCastExprClass:
            K = CXCursor_CXXConstCastExpr;
            break;

        case Stmt::CXXFunctionalCastExprClass:
            K = CXCursor_CXXFunctionalCastExpr;
            break;

        case Stmt::CXXAddrspaceCastExprClass:
            K = CXCursor_CXXAddrspaceCastExpr;
            break;

        case Stmt::CXXTypeidExprClass:
            K = CXCursor_CXXTypeidExpr;
            break;

        case Stmt::CXXBoolLiteralExprClass:
            K = CXCursor_CXXBoolLiteralExpr;
            break;

        case Stmt::CXXNullPtrLiteralExprClass:
            K = CXCursor_CXXNullPtrLiteralExpr;
            break;

        case Stmt::CXXThisExprClass:
            K = CXCursor_CXXThisExpr;
            break;

        case Stmt::CXXThrowExprClass:
            K = CXCursor_CXXThrowExpr;
            break;

        case Stmt::CXXNewExprClass:
            K = CXCursor_CXXNewExpr;
            break;

        case Stmt::CXXDeleteExprClass:
            K = CXCursor_CXXDeleteExpr;
            break;

        case Stmt::ObjCStringLiteralClass:
            K = CXCursor_ObjCStringLiteral;
            break;

        case Stmt::ObjCEncodeExprClass:
            K = CXCursor_ObjCEncodeExpr;
            break;

        case Stmt::ObjCSelectorExprClass:
            K = CXCursor_ObjCSelectorExpr;
            break;

        case Stmt::ObjCProtocolExprClass:
            K = CXCursor_ObjCProtocolExpr;
            break;

        case Stmt::ObjCBoolLiteralExprClass:
            K = CXCursor_ObjCBoolLiteralExpr;
            break;

        case Stmt::ObjCAvailabilityCheckExprClass:
            K = CXCursor_ObjCAvailabilityCheckExpr;
            break;

        case Stmt::ObjCBridgedCastExprClass:
            K = CXCursor_ObjCBridgedCastExpr;
            break;

        case Stmt::BlockExprClass:
            K = CXCursor_BlockExpr;
            break;

        case Stmt::PackExpansionExprClass:
            K = CXCursor_PackExpansionExpr;
            break;

        case Stmt::SizeOfPackExprClass:
            K = CXCursor_SizeOfPackExpr;
            break;

        case Stmt::DeclRefExprClass:
            if (const ImplicitParamDecl* IPD =
                dyn_cast_or_null<ImplicitParamDecl>(cast<DeclRefExpr>(S)->getDecl())) {
                if (const ObjCMethodDecl* MD =
                    dyn_cast<ObjCMethodDecl>(IPD->getDeclContext())) {
                    if (MD->getSelfDecl() == IPD) {
                        K = CXCursor_ObjCSelfExpr;
                        break;
                    }
                }
            }

            K = CXCursor_DeclRefExpr;
            break;

        case Stmt::DependentScopeDeclRefExprClass:
        case Stmt::SubstNonTypeTemplateParmExprClass:
        case Stmt::SubstNonTypeTemplateParmPackExprClass:
        case Stmt::FunctionParmPackExprClass:
        case Stmt::UnresolvedLookupExprClass:
        case Stmt::TypoExprClass: // A typo could actually be a DeclRef or a MemberRef
            K = CXCursor_DeclRefExpr;
            break;

        case Stmt::CXXDependentScopeMemberExprClass:
        case Stmt::CXXPseudoDestructorExprClass:
        case Stmt::MemberExprClass:
        case Stmt::MSPropertyRefExprClass:
        case Stmt::ObjCIsaExprClass:
        case Stmt::ObjCIvarRefExprClass:
        case Stmt::ObjCPropertyRefExprClass:
        case Stmt::UnresolvedMemberExprClass:
            K = CXCursor_MemberRefExpr;
            break;

        case Stmt::CallExprClass:
        case Stmt::CXXOperatorCallExprClass:
        case Stmt::CXXMemberCallExprClass:
        case Stmt::CUDAKernelCallExprClass:
        case Stmt::CXXConstructExprClass:
        case Stmt::CXXInheritedCtorInitExprClass:
        case Stmt::CXXTemporaryObjectExprClass:
        case Stmt::CXXUnresolvedConstructExprClass:
        case Stmt::UserDefinedLiteralClass:
            K = CXCursor_CallExpr;
            break;

        case Stmt::LambdaExprClass:
            K = CXCursor_LambdaExpr;
            break;

        case Stmt::ObjCMessageExprClass: {
            K = CXCursor_ObjCMessageExpr;
            int SelectorIdIndex = -1;
            // Check if cursor points to a selector id.
            if (RegionOfInterest.isValid() &&
                RegionOfInterest.getBegin() == RegionOfInterest.getEnd()) {
                SmallVector<SourceLocation, 16> SelLocs;
                cast<ObjCMessageExpr>(S)->getSelectorLocs(SelLocs);
                SmallVectorImpl<SourceLocation>::iterator I =
                    llvm::find(SelLocs, RegionOfInterest.getBegin());
                if (I != SelLocs.end())
                    SelectorIdIndex = I - SelLocs.begin();
            }
            CXCursor C = { K, 0, { Parent, S, TU } };
            return getSelectorIdentifierCursor(SelectorIdIndex, C);
        }

        case Stmt::MSDependentExistsStmtClass:
            K = CXCursor_UnexposedStmt;
            break;
        case Stmt::OMPParallelDirectiveClass:
            K = CXCursor_OMPParallelDirective;
            break;
        case Stmt::OMPSimdDirectiveClass:
            K = CXCursor_OMPSimdDirective;
            break;
        case Stmt::OMPForDirectiveClass:
            K = CXCursor_OMPForDirective;
            break;
        case Stmt::OMPForSimdDirectiveClass:
            K = CXCursor_OMPForSimdDirective;
            break;
        case Stmt::OMPSectionsDirectiveClass:
            K = CXCursor_OMPSectionsDirective;
            break;
        case Stmt::OMPSectionDirectiveClass:
            K = CXCursor_OMPSectionDirective;
            break;
        case Stmt::OMPSingleDirectiveClass:
            K = CXCursor_OMPSingleDirective;
            break;
        case Stmt::OMPMasterDirectiveClass:
            K = CXCursor_OMPMasterDirective;
            break;
        case Stmt::OMPCriticalDirectiveClass:
            K = CXCursor_OMPCriticalDirective;
            break;
        case Stmt::OMPParallelForDirectiveClass:
            K = CXCursor_OMPParallelForDirective;
            break;
        case Stmt::OMPParallelForSimdDirectiveClass:
            K = CXCursor_OMPParallelForSimdDirective;
            break;
        case Stmt::OMPParallelMasterDirectiveClass:
            K = CXCursor_OMPParallelMasterDirective;
            break;
        case Stmt::OMPParallelSectionsDirectiveClass:
            K = CXCursor_OMPParallelSectionsDirective;
            break;
        case Stmt::OMPTaskDirectiveClass:
            K = CXCursor_OMPTaskDirective;
            break;
        case Stmt::OMPTaskyieldDirectiveClass:
            K = CXCursor_OMPTaskyieldDirective;
            break;
        case Stmt::OMPBarrierDirectiveClass:
            K = CXCursor_OMPBarrierDirective;
            break;
        case Stmt::OMPTaskwaitDirectiveClass:
            K = CXCursor_OMPTaskwaitDirective;
            break;
        case Stmt::OMPTaskgroupDirectiveClass:
            K = CXCursor_OMPTaskgroupDirective;
            break;
        case Stmt::OMPFlushDirectiveClass:
            K = CXCursor_OMPFlushDirective;
            break;
        case Stmt::OMPDepobjDirectiveClass:
            K = CXCursor_OMPDepobjDirective;
            break;
        case Stmt::OMPScanDirectiveClass:
            K = CXCursor_OMPScanDirective;
            break;
        case Stmt::OMPOrderedDirectiveClass:
            K = CXCursor_OMPOrderedDirective;
            break;
        case Stmt::OMPAtomicDirectiveClass:
            K = CXCursor_OMPAtomicDirective;
            break;
        case Stmt::OMPTargetDirectiveClass:
            K = CXCursor_OMPTargetDirective;
            break;
        case Stmt::OMPTargetDataDirectiveClass:
            K = CXCursor_OMPTargetDataDirective;
            break;
        case Stmt::OMPTargetEnterDataDirectiveClass:
            K = CXCursor_OMPTargetEnterDataDirective;
            break;
        case Stmt::OMPTargetExitDataDirectiveClass:
            K = CXCursor_OMPTargetExitDataDirective;
            break;
        case Stmt::OMPTargetParallelDirectiveClass:
            K = CXCursor_OMPTargetParallelDirective;
            break;
        case Stmt::OMPTargetParallelForDirectiveClass:
            K = CXCursor_OMPTargetParallelForDirective;
            break;
        case Stmt::OMPTargetUpdateDirectiveClass:
            K = CXCursor_OMPTargetUpdateDirective;
            break;
        case Stmt::OMPTeamsDirectiveClass:
            K = CXCursor_OMPTeamsDirective;
            break;
        case Stmt::OMPCancellationPointDirectiveClass:
            K = CXCursor_OMPCancellationPointDirective;
            break;
        case Stmt::OMPCancelDirectiveClass:
            K = CXCursor_OMPCancelDirective;
            break;
        case Stmt::OMPTaskLoopDirectiveClass:
            K = CXCursor_OMPTaskLoopDirective;
            break;
        case Stmt::OMPTaskLoopSimdDirectiveClass:
            K = CXCursor_OMPTaskLoopSimdDirective;
            break;
        case Stmt::OMPMasterTaskLoopDirectiveClass:
            K = CXCursor_OMPMasterTaskLoopDirective;
            break;
        case Stmt::OMPMasterTaskLoopSimdDirectiveClass:
            K = CXCursor_OMPMasterTaskLoopSimdDirective;
            break;
        case Stmt::OMPParallelMasterTaskLoopDirectiveClass:
            K = CXCursor_OMPParallelMasterTaskLoopDirective;
            break;
        case Stmt::OMPParallelMasterTaskLoopSimdDirectiveClass:
            K = CXCursor_OMPParallelMasterTaskLoopSimdDirective;
            break;
        case Stmt::OMPDistributeDirectiveClass:
            K = CXCursor_OMPDistributeDirective;
            break;
        case Stmt::OMPDistributeParallelForDirectiveClass:
            K = CXCursor_OMPDistributeParallelForDirective;
            break;
        case Stmt::OMPDistributeParallelForSimdDirectiveClass:
            K = CXCursor_OMPDistributeParallelForSimdDirective;
            break;
        case Stmt::OMPDistributeSimdDirectiveClass:
            K = CXCursor_OMPDistributeSimdDirective;
            break;
        case Stmt::OMPTargetParallelForSimdDirectiveClass:
            K = CXCursor_OMPTargetParallelForSimdDirective;
            break;
        case Stmt::OMPTargetSimdDirectiveClass:
            K = CXCursor_OMPTargetSimdDirective;
            break;
        case Stmt::OMPTeamsDistributeDirectiveClass:
            K = CXCursor_OMPTeamsDistributeDirective;
            break;
        case Stmt::OMPTeamsDistributeSimdDirectiveClass:
            K = CXCursor_OMPTeamsDistributeSimdDirective;
            break;
        case Stmt::OMPTeamsDistributeParallelForSimdDirectiveClass:
            K = CXCursor_OMPTeamsDistributeParallelForSimdDirective;
            break;
        case Stmt::OMPTeamsDistributeParallelForDirectiveClass:
            K = CXCursor_OMPTeamsDistributeParallelForDirective;
            break;
        case Stmt::OMPTargetTeamsDirectiveClass:
            K = CXCursor_OMPTargetTeamsDirective;
            break;
        case Stmt::OMPTargetTeamsDistributeDirectiveClass:
            K = CXCursor_OMPTargetTeamsDistributeDirective;
            break;
        case Stmt::OMPTargetTeamsDistributeParallelForDirectiveClass:
            K = CXCursor_OMPTargetTeamsDistributeParallelForDirective;
            break;
        case Stmt::OMPTargetTeamsDistributeParallelForSimdDirectiveClass:
            K = CXCursor_OMPTargetTeamsDistributeParallelForSimdDirective;
            break;
        case Stmt::OMPTargetTeamsDistributeSimdDirectiveClass:
            K = CXCursor_OMPTargetTeamsDistributeSimdDirective;
            break;
        case Stmt::BuiltinBitCastExprClass:
            K = CXCursor_BuiltinBitCastExpr;
        }

        CXCursor C = { K, 0, { Parent, S, TU } };
        return C;
    }

    CXCursor MakeCXCursor(const CXXBaseSpecifier* B, CXTranslationUnit TU) {
        CXCursor C = { CXCursor_CXXBaseSpecifier, 0, { B, nullptr, TU } };
        return C;
    }
}
