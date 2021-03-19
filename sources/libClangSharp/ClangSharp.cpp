// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

#include "ClangSharp.h"
#include "CXCursor.h"
#include "CXLoadedDiagnostic.h"
#include "CXSourceLocation.h"
#include "CXString.h"
#include "CXTranslationUnit.h"
#include "CXType.h"

#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)

#include <clang/Basic/SourceManager.h>

#pragma warning(pop)

using namespace clang;
using namespace clang::cxcursor;
using namespace clang::cxloc;
using namespace clang::cxstring;
using namespace clang::cxtu;
using namespace clang::cxtype;

CX_TemplateArgument MakeCXTemplateArgument(const TemplateArgument* TA, CXCursor Parent, CXTranslationUnit TU) {
    if (!TA) {
        return { };
    }

    assert(TA && !clang_Cursor_isNull(C) && TU && "Invalid arguments!");

    CX_TemplateArgument T;

    T.kind = static_cast<CXTemplateArgumentKind>(TA->getKind() + 1);
    T.xdata = 1;
    T.parentCursor = Parent;
    T.value = TA;
    T.tu = TU;

    return T;
}

CX_TemplateArgument MakeCXTemplateArgument(const TemplateArgument* TA, CXType Parent, CXTranslationUnit TU) {
    CX_TemplateArgument T;

    if (TA) {
        T.kind = static_cast<CXTemplateArgumentKind>(TA->getKind() + 1);
        T.xdata = 2;
        T.parentType = Parent;
    }

    T.value = TA;
    T.tu = TU;

    return T;
}

CX_TemplateArgumentLoc MakeCXTemplateArgumentLoc(const TemplateArgumentLoc* TAL, CXCursor Parent, CXTranslationUnit TU) {
    CX_TemplateArgumentLoc T;

    if (TAL) {
        T.xdata = 1;
        T.parentCursor = Parent;
    }

    T.value = TAL;
    T.tu = TU;

    return T;
}

CXCursor clangsharp_Cursor_getArgument(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            if (i < BD->getNumParams()) {
                return MakeCXCursor(BD->parameters()[i], getCursorTU(C));
            }
        }

        if (const CapturedDecl* CD = dyn_cast<CapturedDecl>(D)) {
            if (i < CD->getNumParams()) {
                return MakeCXCursor(CD->parameters()[i], getCursorTU(C));
            }
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CastExpr* CE = dyn_cast<CastExpr>(E)) {

            unsigned n = 0;

            for (auto path : CE->path()) {
                if (n == i) {
                    return MakeCXCursor(path, getCursorTU(C));
                }
                n++;
            }
        }

        if (const CXXUnresolvedConstructExpr* CXXUCE = dyn_cast<CXXUnresolvedConstructExpr>(E)) {
            if (i < CXXUCE->arg_size()) {
                return MakeCXCursor(CXXUCE->getArg(i), getCursorDecl(C), getCursorTU(C));
            }
        }

        if (const ExprWithCleanups* EWC = dyn_cast<ExprWithCleanups>(E)) {
            if (i < EWC->getNumObjects()) {
                llvm::PointerUnion<BlockDecl*, CompoundLiteralExpr*> object = EWC->getObject(i);

                if (object.is<BlockDecl*>()) {
                    return MakeCXCursor(object.get<BlockDecl*>(), getCursorTU(C));
                }
                else {
                    return MakeCXCursor(object.get<CompoundLiteralExpr*>(), getCursorDecl(C), getCursorTU(C));
                }
            }
        }
    }

    return clang_Cursor_getArgument(C, i);
}

CXType clangsharp_Cursor_getArgumentType(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const UnaryExprOrTypeTraitExpr* UETTE = dyn_cast<UnaryExprOrTypeTraitExpr>(E)) {
            if (UETTE->isArgumentType()) {
                return MakeCXType(UETTE->getArgumentType(), getCursorTU(C));
            }
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

int64_t clangsharp_Cursor_getArraySize(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const ArrayInitLoopExpr* AILE = dyn_cast<ArrayInitLoopExpr>(E)) {
            return AILE->getArraySize().getSExtValue();
        }
    }

    return -1;
}

CXCursor clangsharp_Cursor_getAssociatedConstraint(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassTemplatePartialSpecializationDecl* CTPSD = dyn_cast<ClassTemplatePartialSpecializationDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            CTPSD->getAssociatedConstraints(associatedConstraints);

            if (i < associatedConstraints.size()) {
                return MakeCXCursor(associatedConstraints[i], CTPSD, getCursorTU(C));
            }
        }

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            NTTPD->getAssociatedConstraints(associatedConstraints);

            if (i < associatedConstraints.size()) {
                return MakeCXCursor(associatedConstraints[i], NTTPD, getCursorTU(C));
            }
        }

        if (const TemplateDecl* TD = dyn_cast<TemplateDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            TD->getAssociatedConstraints(associatedConstraints);

            if (i < associatedConstraints.size()) {
                return MakeCXCursor(associatedConstraints[i], TD, getCursorTU(C));
            }
        }

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            TTPD->getAssociatedConstraints(associatedConstraints);

            if (i < associatedConstraints.size()) {
                return MakeCXCursor(associatedConstraints[i], TTPD, getCursorTU(C));
            }
        }

        if (const VarTemplatePartialSpecializationDecl* VTPSD = dyn_cast<VarTemplatePartialSpecializationDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            VTPSD->getAssociatedConstraints(associatedConstraints);

            if (i < associatedConstraints.size()) {
                return MakeCXCursor(associatedConstraints[i], VTPSD, getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getAsFunction(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getAsFunction(), getCursorTU(C));
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getAttr(CXCursor C, unsigned i) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const AttributedStmt* AS = dyn_cast<AttributedStmt>(S)) {
            ArrayRef<const Attr*> attrs = AS->getAttrs();

            if (i < attrs.size()) {
                return MakeCXCursor(attrs[i], getCursorDecl(C), getCursorTU(C));
            }
        }
    }

    return clang_Cursor_getArgument(C, i);
}

CX_AttrKind clangsharp_Cursor_getAttrKind(CXCursor C) {
    if (clang_isAttribute(C.kind)) {
        const Attr* A = getCursorAttr(C);
        return static_cast<CX_AttrKind>(A->getKind() + 1);
    }

    return CX_AttrKind_Invalid;
}

CX_BinaryOperatorKind clangsharp_Cursor_getBinaryOpcode(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const BinaryOperator* BO = dyn_cast<BinaryOperator>(E)) {
            return static_cast<CX_BinaryOperatorKind>(BO->getOpcode() + 1);
        }

        if (const CXXFoldExpr* CFE = dyn_cast<CXXFoldExpr>(E)) {
            return static_cast<CX_BinaryOperatorKind>(CFE->getOperator() + 1);
        }

        if (const CXXRewrittenBinaryOperator* CRBO = dyn_cast<CXXRewrittenBinaryOperator>(E)) {
            return static_cast<CX_BinaryOperatorKind>(CRBO->getOperator() + 1);
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

CXCursor clangsharp_Cursor_getBinding(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BindingDecl* BD = dyn_cast<BindingDecl>(D)) {
            return MakeCXCursor(BD->getBinding(), BD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getBitWidth(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FieldDecl* FD = dyn_cast<FieldDecl>(D)) {
            return MakeCXCursor(FD->getBitWidth(), FD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getBlockManglingContextDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return MakeCXCursor(BD->getBlockManglingContextDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

int clangsharp_Cursor_getBlockManglingNumber(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->getBlockManglingNumber();
        }
    }

    return -1;
}

unsigned clangsharp_Cursor_getBlockMissingReturnType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->blockMissingReturnType();
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getBody(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getBody(), D, getCursorTU(C));
    }

    return clang_getNullCursor();
}

CXType clangsharp_Cursor_getCallResultType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return MakeCXType(FD->getCallResultType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

unsigned clangsharp_Cursor_getCanAvoidCopyToHeap(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->canAvoidCopyToHeap();
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getCaptureCopyExpr(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            ArrayRef<BlockDecl::Capture> captures = BD->captures();

            if (i < captures.size()) {
                return MakeCXCursor(captures[i].getCopyExpr(), BD, getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getCapturedVar(CXCursor C, unsigned i) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const CapturedStmt* CS = dyn_cast<CapturedStmt>(S)) {
            unsigned n = 0;

            for (auto capture : CS->captures()) {
                if (n == i) {
                    return MakeCXCursor(capture.getCapturedVar(), getCursorTU(C));
                }
                n++;
            }
        }
    }

    return clang_getNullCursor();
}

CX_VariableCaptureKind clangsharp_Cursor_getCaptureKind(CXCursor C, unsigned i) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const CapturedStmt* CS = dyn_cast<CapturedStmt>(S)) {
            unsigned n = 0;

            for (auto capture : CS->captures()) {
                if (n == i) {
                    return static_cast<CX_VariableCaptureKind>(capture.getCaptureKind() + 1);
                }
                n++;
            }
        }
    }

    return CX_VCK_Invalid;
}

unsigned clangsharp_Cursor_getCaptureHasCopyExpr(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            ArrayRef<BlockDecl::Capture> captures = BD->captures();

            if (i < captures.size()) {
                return captures[i].hasCopyExpr();
            }
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCaptureIsByRef(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            ArrayRef<BlockDecl::Capture> captures = BD->captures();

            if (i < captures.size()) {
                return captures[i].isByRef();
            }
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCaptureIsEscapingByRef(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            ArrayRef<BlockDecl::Capture> captures = BD->captures();

            if (i < captures.size()) {
                return captures[i].isEscapingByref();
            }
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCaptureIsNested(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            ArrayRef<BlockDecl::Capture> captures = BD->captures();

            if (i < captures.size()) {
                return captures[i].isNested();
            }
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCaptureIsNonEscapingByRef(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            ArrayRef<BlockDecl::Capture> captures = BD->captures();

            if (i < captures.size()) {
                return captures[i].isNonEscapingByref();
            }
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getCapturedDecl(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const CapturedStmt* CS = dyn_cast<CapturedStmt>(S)) {
            return MakeCXCursor(CS->getCapturedDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getCapturedRecordDecl(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const CapturedStmt* CS = dyn_cast<CapturedStmt>(S)) {
            return MakeCXCursor(CS->getCapturedRecordDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CX_CapturedRegionKind clangsharp_Cursor_getCapturedRegionKind(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const CapturedStmt* CS = dyn_cast<CapturedStmt>(S)) {
            return static_cast<CX_CapturedRegionKind>(CS->getCapturedRegionKind() + 1);
        }
    }

    return CX_CR_Invalid;
}

CXCursor clangsharp_Cursor_getCapturedStmt(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const CapturedStmt* CS = dyn_cast<CapturedStmt>(S)) {
            return MakeCXCursor(CS->getCapturedStmt(), getCursorDecl(C), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

unsigned clangsharp_Cursor_getCapturesCXXThis(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->capturesCXXThis();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCapturesVariable(CXCursor C, CXCursor V) {
    if (clang_isDeclaration(C.kind) && clang_isDeclaration(V.kind)) {
        const BlockDecl* BD = dyn_cast<BlockDecl>(getCursorDecl(C));
        const VarDecl* VD = dyn_cast<VarDecl>(getCursorDecl(V));

        if ((BD != nullptr) && (VD != nullptr)) {
            return BD->capturesVariable(VD);
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getCaptureVariable(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            ArrayRef<BlockDecl::Capture> captures = BD->captures();

            if (i < captures.size()) {
                return MakeCXCursor(captures[i].getVariable(), getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CX_CastKind clangsharp_Cursor_getCastKind(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CastExpr* CastEx = dyn_cast<CastExpr>(E)) {
            return static_cast<CX_CastKind>(CastEx->getCastKind() + 1);
        }
    }

    return CX_CK_Invalid;
}

CX_CharacterKind clangsharp_Cursor_getCharacterLiteralKind(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CharacterLiteral* CL = dyn_cast<CharacterLiteral>(E)) {
            return static_cast<CX_CharacterKind>(CL->getKind() + 1);
        }

        if (const StringLiteral* SL = dyn_cast<StringLiteral>(E)) {
            return static_cast<CX_CharacterKind>(SL->getKind() + 1);
        }
    }

    return CX_CLK_Invalid;
}

unsigned clangsharp_Cursor_getCharacterLiteralValue(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CharacterLiteral* CL = dyn_cast<CharacterLiteral>(E)) {
            return CL->getValue();
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getChild(CXCursor C, unsigned i) {
    if (clang_isExpression(C.kind) || clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        unsigned n = 0;

        for (auto child : S->children()) {
            if (n == i) {
                return MakeCXCursor(child, getCursorDecl(C), getCursorTU(C));
            }
            n++;
        }
    }

    return clang_getNullCursor();
}

CXType clangsharp_Cursor_getComputationLhsType(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CompoundAssignOperator* CAO = dyn_cast<CompoundAssignOperator>(E)) {
            return MakeCXType(CAO->getComputationLHSType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CXType clangsharp_Cursor_getComputationResultType(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CompoundAssignOperator* CAO = dyn_cast<CompoundAssignOperator>(E)) {
            return MakeCXType(CAO->getComputationResultType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CXCursor clangsharp_Cursor_getConstraintExpr(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConceptDecl* CD = dyn_cast<ConceptDecl>(D)) {
            return MakeCXCursor(CD->getConstraintExpr(), CD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getConstructedBaseClass(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConstructorUsingShadowDecl* CUSD = dyn_cast<ConstructorUsingShadowDecl>(D)) {
            return MakeCXCursor(CUSD->getConstructedBaseClass(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getConstructedBaseClassShadowDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConstructorUsingShadowDecl* CUSD = dyn_cast<ConstructorUsingShadowDecl>(D)) {
            return MakeCXCursor(CUSD->getConstructedBaseClassShadowDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CX_ConstructionKind clangsharp_Cursor_getConstructionKind(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXConstructExpr* CXXCE = dyn_cast<CXXConstructExpr>(E)) {
            return static_cast<CX_ConstructionKind>(CXXCE->getConstructionKind() + 1);
        }
    }

    return _CX_CK_Invalid;
}

unsigned clangsharp_Cursor_getConstructsVirtualBase(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConstructorUsingShadowDecl* CUSD = dyn_cast<ConstructorUsingShadowDecl>(D)) {
            return CUSD->constructsVirtualBase();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXInheritedCtorInitExpr* CXXICIE = dyn_cast<CXXInheritedCtorInitExpr>(E)) {
            return CXXICIE->constructsVBase();
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getContextParam(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CapturedDecl* CD = dyn_cast<CapturedDecl>(D)) {
            return MakeCXCursor(CD->getContextParam(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

int clangsharp_Cursor_getContextParamPosition(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CapturedDecl* CD = dyn_cast<CapturedDecl>(D)) {
            return CD->getContextParamPosition();
        }
    }

    return -1;
}

CXCursor clangsharp_Cursor_getConversionFunction(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CastExpr* CE = dyn_cast<CastExpr>(E)) {
            return MakeCXCursor(CE->getConversionFunction(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

unsigned clangsharp_Cursor_getBoolLiteralValue(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXBoolLiteralExpr* CBLE = dyn_cast<CXXBoolLiteralExpr>(E)) {
            return CBLE->getValue();
        }

        if (const CXXNoexceptExpr* CXXNE = dyn_cast<CXXNoexceptExpr>(E)) {
            return CXXNE->getValue();
        }

        if (const ObjCBoolLiteralExpr* OCBLE = dyn_cast<ObjCBoolLiteralExpr>(E)) {
            return OCBLE->getValue();
        }
    }

    return 0;
}

CXType clangsharp_Cursor_getDeclaredReturnType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return MakeCXType(FD->getDeclaredReturnType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CXCursor clangsharp_Cursor_getDecl(CXCursor C, unsigned i) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXNewExpr* CXXNE = dyn_cast<CXXNewExpr>(E)) {
            if (i == 0) {
                return MakeCXCursor(CXXNE->getOperatorDelete(), getCursorTU(C));
            }
            else if (i == 1) {
                return MakeCXCursor(CXXNE->getOperatorNew(), getCursorTU(C));
            }
        }

        if (const FunctionParmPackExpr* FPPE = dyn_cast<FunctionParmPackExpr>(E)) {
            if (i < FPPE->getNumExpansions()) {
                return MakeCXCursor(FPPE->getExpansion(i), getCursorTU(C));
            }
        }

        if (const OverloadExpr* OE = dyn_cast<OverloadExpr>(E)) {
            unsigned n = 0;

            for (auto decl : OE->decls()) {
                if (n == i) {
                    return MakeCXCursor(decl, getCursorTU(C));
                }
                n++;
            }
        }
    }

    return clang_Cursor_getArgument(C, i);
}

CX_DeclKind clangsharp_Cursor_getDeclKind(CXCursor C) {
    if (clang_isDeclaration(C.kind) || clang_isTranslationUnit(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return static_cast<CX_DeclKind>(D->getKind() + 1);
    }

    return CX_DeclKind_Invalid;
}

CXCursor clangsharp_Cursor_getDecomposedDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BindingDecl* BD = dyn_cast<BindingDecl>(D)) {
            return MakeCXCursor(BD->getDecomposedDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getDefaultArg(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            return MakeCXCursor(NTTPD->getDefaultArgument(), NTTPD, getCursorTU(C));
        }

        if (const ParmVarDecl* PVD = dyn_cast<ParmVarDecl>(D)) {
            return MakeCXCursor(PVD->getDefaultArg(), PVD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXType clangsharp_Cursor_getDefaultArgType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            return MakeCXType(TTPD->getDefaultArgument(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CXCursor clangsharp_Cursor_getDependentLambdaCallOperator(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            return MakeCXCursor(CRD->getDependentLambdaCallOperator(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getDescribedClassTemplate(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            return MakeCXCursor(CRD->getDescribedClassTemplate(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getDescribedTemplate(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getDescribedTemplate(), getCursorTU(C));
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getDestructor(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            return MakeCXCursor(CRD->getDestructor(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

unsigned clangsharp_Cursor_getDoesNotEscape(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->doesNotEscape();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getDoesUsualArrayDeleteWantSize(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXNewExpr* CXXNE = dyn_cast<CXXNewExpr>(E)) {
            return CXXNE->doesUsualArrayDeleteWantSize();
        }

        if (const CXXDeleteExpr* CXXDE = dyn_cast<CXXDeleteExpr>(E)) {
            return CXXDE->doesUsualArrayDeleteWantSize();
        }
    }

    return 0;
}

CXType clangsharp_Cursor_getEnumDeclPromotionType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumDecl* ED = dyn_cast<EnumDecl>(D)) {
            return MakeCXType(ED->getPromotionType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CX_ExprDependence clangsharp_Cursor_getExprDependence(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);
        return static_cast<CX_ExprDependence>(E->getDependence());
    }

    return CX_ED_None;
}

int clangsharp_Cursor_getFieldIndex(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FieldDecl* FD = dyn_cast<FieldDecl>(D)) {
            return FD->getFieldIndex();
        }
    }

    return -1;
}

CX_FloatingSemantics clangsharp_Cursor_getFloatingLiteralSemantics(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const FloatingLiteral* FL = dyn_cast<FloatingLiteral>(E)) {
            return static_cast<CX_FloatingSemantics>(FL->getRawSemantics() + 1);
        }
    }

    return CX_FLK_Invalid;
}

double clangsharp_Cursor_getFloatingLiteralValueAsApproximateDouble(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const FloatingLiteral* FL = dyn_cast<FloatingLiteral>(E)) {
            return FL->getValueAsApproximateDouble();
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getFoundDecl(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const DeclRefExpr* DRE = dyn_cast<DeclRefExpr>(E)) {
            return MakeCXCursor(DRE->getFoundDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getFriendDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FriendDecl* FD = dyn_cast<FriendDecl>(D)) {
            return MakeCXCursor(FD->getFriendDecl(), getCursorTU(C));
        }

        if (const FriendTemplateDecl* FTD = dyn_cast<FriendTemplateDecl>(D)) {
            return MakeCXCursor(FTD->getFriendDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

int clangsharp_Cursor_getFunctionScopeDepth(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ParmVarDecl* PVD = dyn_cast<ParmVarDecl>(D)) {
            return PVD->getFunctionScopeDepth();
        }
    }

    return -1;
}

int clangsharp_Cursor_getFunctionScopeIndex(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ParmVarDecl* PVD = dyn_cast<ParmVarDecl>(D)) {
            return PVD->getFunctionScopeIndex();
        }
    }

    return -1;
}

MSGuidDeclParts clangsharp_Cursor_getGuidValue(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const MSGuidDecl* MSGD = dyn_cast<MSGuidDecl>(D)) {
            return MSGD->getParts();
        }
    }

    return MSGuidDeclParts();
}

unsigned clangsharp_Cursor_getHadMultipleCandidates(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXConstructExpr* CXXCE = dyn_cast<CXXConstructExpr>(E)) {
            return CXXCE->hadMultipleCandidates();
        }

        if (const DeclRefExpr* DRE = dyn_cast<DeclRefExpr>(E)) {
            return DRE->hadMultipleCandidates();
        }

        if (const MemberExpr* ME = dyn_cast<MemberExpr>(E)) {
            return ME->hadMultipleCandidates();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasBody(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->hasBody();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasDefaultArg(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            return NTTPD->hasDefaultArgument();
        }

        if (const ParmVarDecl* PVD = dyn_cast<ParmVarDecl>(D)) {
            return PVD->hasDefaultArg();
        }

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            return TTPD->hasDefaultArgument();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasElseStorage(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const IfStmt* IS = dyn_cast<IfStmt>(S)) {
            return IS->hasElseStorage();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasExplicitTemplateArgs(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            return CSFSD->hasExplicitTemplateArgs();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDependentScopeMemberExpr* CXXDSME = dyn_cast<CXXDependentScopeMemberExpr>(E)) {
            return CXXDSME->hasExplicitTemplateArgs();
        }

        if (const DeclRefExpr* DRE = dyn_cast<DeclRefExpr>(E)) {
            return DRE->hasExplicitTemplateArgs();
        }

        if (const DependentScopeDeclRefExpr* DSDRE = dyn_cast<DependentScopeDeclRefExpr>(E)) {
            return DSDRE->hasExplicitTemplateArgs();
        }

        if (const MemberExpr* ME = dyn_cast<MemberExpr>(E)) {
            return ME->hasExplicitTemplateArgs();
        }

        if (const OverloadExpr* OE = dyn_cast<OverloadExpr>(E)) {
            return OE->hasExplicitTemplateArgs();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasExternalStorage(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return VD->hasExternalStorage();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasGlobalStorage(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return VD->hasGlobalStorage();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasImplicitReturnZero(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->hasImplicitReturnZero();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasInheritedDefaultArg(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            return NTTPD->defaultArgumentWasInherited();
        }

        if (const ParmVarDecl* PVD = dyn_cast<ParmVarDecl>(D)) {
            return PVD->hasInheritedDefaultArg();
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            return TTPD->defaultArgumentWasInherited();
        }

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            return TTPD->defaultArgumentWasInherited();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasInit(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return VD->hasInit();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXNewExpr* CXXNE = dyn_cast<CXXNewExpr>(E)) {
            return CXXNE->hasInitializer();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasInitStorage(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const IfStmt* IS = dyn_cast<IfStmt>(S)) {
            return IS->hasInitStorage();
        }

        if (const SwitchStmt* SS = dyn_cast<SwitchStmt>(S)) {
            return SS->hasInitStorage();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasLeadingEmptyMacro(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const NullStmt* NS = dyn_cast<NullStmt>(S)) {
            return NS->hasLeadingEmptyMacro();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasLocalStorage(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return VD->hasLocalStorage();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasPlaceholderTypeConstraint(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            return NTTPD->hasPlaceholderTypeConstraint();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getHasTemplateKeyword(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDependentScopeMemberExpr* CXXDSME = dyn_cast<CXXDependentScopeMemberExpr>(E)) {
            return CXXDSME->hasTemplateKeyword();
        }

        if (const DeclRefExpr* DRE = dyn_cast<DeclRefExpr>(E)) {
            return DRE->hasTemplateKeyword();
        }

        if (const DependentScopeDeclRefExpr* DSDRE = dyn_cast<DependentScopeDeclRefExpr>(E)) {
            return DSDRE->hasTemplateKeyword();
        }

        if (const MemberExpr* ME = dyn_cast<MemberExpr>(E)) {
            return ME->hasTemplateKeyword();
        }

        if (const OverloadExpr* OE = dyn_cast<OverloadExpr>(E)) {
            return OE->hasTemplateKeyword();
        }
    }

    return 0;
}


unsigned clangsharp_Cursor_getHasVarStorage(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const IfStmt* IS = dyn_cast<IfStmt>(S)) {
            return IS->hasVarStorage();
        }

        if (const SwitchStmt* SS = dyn_cast<SwitchStmt>(S)) {
            return SS->hasVarStorage();
        }

        if (const WhileStmt* WS = dyn_cast<WhileStmt>(S)) {
            return WS->hasVarStorage();
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getHoldingVar(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BindingDecl* BD = dyn_cast<BindingDecl>(D)) {
            return MakeCXCursor(BD->getHoldingVar(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getInClassInitializer(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FieldDecl* FD = dyn_cast<FieldDecl>(D)) {
            return MakeCXCursor(FD->getInClassInitializer(), FD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

unsigned clangsharp_Cursor_getInheritedFromVBase(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXInheritedCtorInitExpr* CXXICIE = dyn_cast<CXXInheritedCtorInitExpr>(E)) {
            return CXXICIE->inheritedFromVBase();
        }
    }

    return 0;
}

CXCursor clangsharp_Cursor_getInitExpr(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return MakeCXCursor(ECD->getInitExpr(), ECD, getCursorTU(C));
        }

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return MakeCXCursor(VD->getInit(), VD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getInstantiatedFromMember(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassTemplatePartialSpecializationDecl* CTPSD = dyn_cast<ClassTemplatePartialSpecializationDecl>(D)) {
            return MakeCXCursor(CTPSD->getInstantiatedFromMemberTemplate(), getCursorTU(C));
        }

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            return MakeCXCursor(CRD->getInstantiatedFromMemberClass(), getCursorTU(C));
        }

        if (const EnumDecl* ED = dyn_cast<EnumDecl>(D)) {
            return MakeCXCursor(ED->getInstantiatedFromMemberEnum(), getCursorTU(C));
        }

        if (const RedeclarableTemplateDecl* RTD = dyn_cast<RedeclarableTemplateDecl>(D)) {
            return MakeCXCursor(RTD->getInstantiatedFromMemberTemplate(), getCursorTU(C));
        }

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return MakeCXCursor(VD->getInstantiatedFromStaticDataMember(), getCursorTU(C));
        }

        if (const VarTemplatePartialSpecializationDecl* VTPSD = dyn_cast<VarTemplatePartialSpecializationDecl>(D)) {
            return MakeCXCursor(VTPSD->getInstantiatedFromMember(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

int64_t clangsharp_Cursor_getIntegerLiteralValue(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const IntegerLiteral* IL = dyn_cast<IntegerLiteral>(E)) {
            return IL->getValue().getSExtValue();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsAllEnumCasesCovered(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const SwitchStmt* SS = dyn_cast<SwitchStmt>(S)) {
            return SS->isAllEnumCasesCovered();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsAlwaysNull(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDynamicCastExpr* CXXDCE = dyn_cast<CXXDynamicCastExpr>(E)) {
            return CXXDCE->isAlwaysNull();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsAnonymousStructOrUnion(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FieldDecl* FD = dyn_cast<FieldDecl>(D)) {
            return FD->isAnonymousStructOrUnion();
        }

        if (const RecordDecl* RD = dyn_cast<RecordDecl>(D)) {
            return RD->isAnonymousStructOrUnion();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsArgumentType(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const UnaryExprOrTypeTraitExpr* UETTE = dyn_cast<UnaryExprOrTypeTraitExpr>(E)) {
            return UETTE->isArgumentType();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsArrayForm(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDeleteExpr* CXXDE = dyn_cast<CXXDeleteExpr>(E)) {
            return CXXDE->isArrayForm();
        }

        if (const CXXNewExpr* CXXNE = dyn_cast<CXXNewExpr>(E)) {
            return CXXNE->isArray();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsArrayFormAsWritten(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDeleteExpr* CXXDE = dyn_cast<CXXDeleteExpr>(E)) {
            return CXXDE->isArrayFormAsWritten();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsArrow(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDependentScopeMemberExpr* CXXDSME = dyn_cast<CXXDependentScopeMemberExpr>(E)) {
            return CXXDSME->isArrow();
        }

        if (const MemberExpr* ME = dyn_cast<MemberExpr>(E)) {
            return ME->isArrow();
        }

        if (const ObjCIsaExpr* OCIE = dyn_cast<ObjCIsaExpr>(E)) {
            return OCIE->isArrow();
        }

        if (const ObjCIvarRefExpr* OCIRE = dyn_cast<ObjCIvarRefExpr>(E)) {
            return OCIRE->isArrow();
        }

        if (const CXXPseudoDestructorExpr* CXXPSDE = dyn_cast<CXXPseudoDestructorExpr>(E)) {
            return CXXPSDE->isArrow();
        }

        if (const UnresolvedMemberExpr* UME = dyn_cast<UnresolvedMemberExpr>(E)) {
            return UME->isArrow();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsConditionTrue(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const ChooseExpr* CE = dyn_cast<ChooseExpr>(E)) {
            if (!CE->isConditionDependent()) {
                return CE->isConditionTrue();
            }
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsConstexpr(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const IfStmt* IS = dyn_cast<IfStmt>(S)) {
            return IS->isConstexpr();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsConversionFromLambda(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->isConversionFromLambda();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsCXXTry(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const SEHTryStmt* SEHTS = dyn_cast<SEHTryStmt>(S)) {
            return SEHTS->getIsCXXTry();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsDefined(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isDefined();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsDeprecated(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return D->isDeprecated();
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsExternC(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isExternC();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsExpandedParameterPack(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            return TTPD->isExpandedParameterPack();
        }

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            return TTPD->isExpandedParameterPack();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsFileScope(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CompoundLiteralExpr* CLE = dyn_cast<CompoundLiteralExpr>(E)) {
            return CLE->isFileScope();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsGlobal(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isGlobal();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXNewExpr* CXXNE = dyn_cast<CXXNewExpr>(E)) {
            return CXXNE->isGlobalNew();
        }

        if (const CXXDeleteExpr* CXXDE = dyn_cast<CXXDeleteExpr>(E)) {
            return CXXDE->isGlobalDelete();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsIfExists(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const MSDependentExistsStmt* MSDES = dyn_cast<MSDependentExistsStmt>(S)) {
            return MSDES->isIfExists();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsImplicit(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXThisExpr* CXXTE = dyn_cast<CXXThisExpr>(E)) {
            return CXXTE->isImplicit();
        }

        if (const ImplicitCastExpr* ICE = dyn_cast<ImplicitCastExpr>(E)) {
            return !ICE->isPartOfExplicitCast();
        }

        if (const InitListExpr* ILE = dyn_cast<InitListExpr>(E)) {
            return !ILE->isExplicit();
        }

        if (const MemberExpr* ME = dyn_cast<MemberExpr>(E)) {
            return ME->isImplicitAccess();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsIncomplete(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const MatrixSubscriptExpr* MSE = dyn_cast<MatrixSubscriptExpr>(E)) {
            return MSE->isIncomplete();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsListInitialization(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXConstructExpr* CXXCE = dyn_cast<CXXConstructExpr>(E)) {
            return CXXCE->isListInitialization();
        }

        if (const CXXFunctionalCastExpr* CXXFCE = dyn_cast<CXXFunctionalCastExpr>(E)) {
            return CXXFCE->isListInitialization();
        }

        if (const CXXUnresolvedConstructExpr* CXXUCE = dyn_cast<CXXUnresolvedConstructExpr>(E)) {
            return CXXUCE->isListInitialization();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsLocalVarDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return VD->isLocalVarDecl();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsLocalVarDeclOrParm(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return VD->isLocalVarDeclOrParm();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsMemberSpecialization(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const RedeclarableTemplateDecl* RTD = dyn_cast<RedeclarableTemplateDecl>(D)) {
            return RTD->isMemberSpecialization();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsNegative(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return ECD->getInitVal().isNegative();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const IntegerLiteral* IL = dyn_cast<IntegerLiteral>(E)) {
            return IL->getValue().isNegative();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsNonNegative(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return ECD->getInitVal().isNonNegative();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const IntegerLiteral* IL = dyn_cast<IntegerLiteral>(E)) {
            return IL->getValue().isNonNegative();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsNoReturn(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isNoReturn();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsNothrow(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CapturedDecl* CD = dyn_cast<CapturedDecl>(D)) {
            return CD->isNothrow();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsOverloadedOperator(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isOverloadedOperator();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsPackExpansion(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            return NTTPD->isPackExpansion();
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            return TTPD->isPackExpansion();
        }

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            return TTPD->isPackExpansion();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsParameterPack(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            return NTTPD->isParameterPack();
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            return TTPD->isParameterPack();
        }

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            return TTPD->isParameterPack();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsPartiallySubstituted(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const SizeOfPackExpr* SOPE = dyn_cast<SizeOfPackExpr>(E)) {
            return SOPE->isPartiallySubstituted();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsPotentiallyEvaluated(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXTypeidExpr* CXXTE = dyn_cast<CXXTypeidExpr>(E)) {
            return CXXTE->isPotentiallyEvaluated();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsPure(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isPure();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsResultDependent(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const GenericSelectionExpr* GSE = dyn_cast<GenericSelectionExpr>(E)) {
            return GSE->isResultDependent();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsReversed(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXRewrittenBinaryOperator* CXXRBO = dyn_cast<CXXRewrittenBinaryOperator>(E)) {
            return CXXRBO->isReversed();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsSigned(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return ECD->getInitVal().isSigned();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsStatic(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isStatic();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsStaticDataMember(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const VarDecl* VD = dyn_cast<VarDecl>(D)) {
            return VD->isStaticDataMember();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsStdInitListInitialization(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXConstructExpr* CXXCE = dyn_cast<CXXConstructExpr>(E)) {
            return CXXCE->isStdInitListInitialization();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsStrictlyPositive(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return ECD->getInitVal().isStrictlyPositive();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const IntegerLiteral* IL = dyn_cast<IntegerLiteral>(E)) {
            return IL->getValue().isStrictlyPositive();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsTemplated(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return D->isTemplated();
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsThisDeclarationADefinition(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassTemplateDecl* CTD = dyn_cast<ClassTemplateDecl>(D)) {
            return CTD->isThisDeclarationADefinition();
        }

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isThisDeclarationADefinition();
        }

        if (const FunctionTemplateDecl* FTD = dyn_cast<FunctionTemplateDecl>(D)) {
            return FTD->isThisDeclarationADefinition();
        }

        if (const TagDecl* TD = dyn_cast<TagDecl>(D)) {
            return TD->isThisDeclarationADefinition();
        }

        if (const VarTemplateDecl* VTD = dyn_cast<VarTemplateDecl>(D)) {
            return VTD->isThisDeclarationADefinition();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsThrownVariableInScope(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXThrowExpr* CXXTE = dyn_cast<CXXThrowExpr>(E)) {
            return CXXTE->isThrownVariableInScope();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsTransparent(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const TypedefNameDecl* TND = dyn_cast<TypedefNameDecl>(D)) {
            return TND->isTransparentTag();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const InitListExpr* ILE = dyn_cast<InitListExpr>(E)) {
            return ILE->isTransparent();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsTypeConcept(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConceptDecl* CTD = dyn_cast<ConceptDecl>(D)) {
            return CTD->isTypeConcept();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsUnavailable(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return D->isUnavailable();
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsUnnamedBitfield(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FieldDecl* FD = dyn_cast<FieldDecl>(D)) {
            return FD->isUnnamedBitfield();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsUnsigned(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return ECD->getInitVal().isUnsigned();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsUnsupportedFriend(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FriendDecl* FD = dyn_cast<FriendDecl>(D)) {
            return FD->isUnsupportedFriend();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getIsVariadic(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->isVariadic();
        }
    }

    return clang_Cursor_isVariadic(C);
}

CXCursor clangsharp_Cursor_getLambdaCallOperator(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            return MakeCXCursor(CRD->getLambdaCallOperator(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getLambdaContextDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            if( CRD->isLambda() )
                return MakeCXCursor(CRD->getLambdaContextDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getLambdaStaticInvoker(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            return MakeCXCursor(CRD->getLambdaStaticInvoker(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getLhsExpr(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXRewrittenBinaryOperator* CRBO = dyn_cast<CXXRewrittenBinaryOperator>(E)) {
            return MakeCXCursor(CRBO->getLHS(), getCursorDecl(C), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

unsigned clangsharp_Cursor_getMaxAlignment(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return D->getMaxAlignment();
    }

    return 0;
}

CXCursor clangsharp_Cursor_getMostRecentDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getMostRecentDecl(), getCursorTU(C));
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getMostRecentNonInjectedDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            return MakeCXCursor(CRD->getMostRecentNonInjectedDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXString clangsharp_Cursor_getName(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDependentScopeMemberExpr* CXXDSME = dyn_cast<CXXDependentScopeMemberExpr>(E)) {
            return createDup(
                CXXDSME->getMemberNameInfo().getAsString());
        }

        if (const CXXNamedCastExpr* CXXNCE = dyn_cast<CXXNamedCastExpr>(E)) {
            return createDup(
                CXXNCE->getCastName());
        }

        if (const DeclRefExpr* DRE = dyn_cast<DeclRefExpr>(E)) {
            return createDup(
                DRE->getNameInfo().getAsString());
        }

        if (const DependentScopeDeclRefExpr* DSDRE = dyn_cast<DependentScopeDeclRefExpr>(E)) {
            return createDup(
                DSDRE->getDeclName().getAsString());
        }

        if (const MemberExpr* ME = dyn_cast<MemberExpr>(E)) {
            return createDup(
                ME->getMemberNameInfo().getAsString());
        }

        if (const OverloadExpr* OE = dyn_cast<OverloadExpr>(E)) {
            return createDup(
                OE->getName().getAsString());
        }

        if (const UnresolvedMemberExpr* UME = dyn_cast<UnresolvedMemberExpr>(E)) {
            return createDup(
                UME->getMemberName().getAsString());
        }
    }

    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const LabelStmt* LS = dyn_cast<LabelStmt>(S)) {
            return createDup(
                LS->getName());
        }

        if (const MSDependentExistsStmt* MSDES = dyn_cast<MSDependentExistsStmt>(S)) {
            return createDup(
                MSDES->getNameInfo().getAsString());
        }
    }

    return createEmpty();
}

CXCursor clangsharp_Cursor_getNextDeclInContext(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getNextDeclInContext(), getCursorTU(C));
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getNextSwitchCase(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const SwitchCase* SC = dyn_cast<SwitchCase>(S)) {
            return MakeCXCursor(SC->getNextSwitchCase(), getCursorDecl(C), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getNominatedBaseClass(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConstructorUsingShadowDecl* CUSD = dyn_cast<ConstructorUsingShadowDecl>(D)) {
            return MakeCXCursor(CUSD->getNominatedBaseClass(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getNominatedBaseClassShadowDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConstructorUsingShadowDecl* CUSD = dyn_cast<ConstructorUsingShadowDecl>(D)) {
            return MakeCXCursor(CUSD->getNominatedBaseClassShadowDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getNonClosureContext(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getNonClosureContext(), getCursorTU(C));
    }

    return clang_getNullCursor();
}

int clangsharp_Cursor_getNumArguments(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->param_size();
        }

        if (const CapturedDecl* CD = dyn_cast<CapturedDecl>(D)) {
            return CD->getNumParams();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CastExpr* CE = dyn_cast<CastExpr>(E)) {
            return CE->path_size();
        }

        if (const CXXUnresolvedConstructExpr* CXXUCE = dyn_cast<CXXUnresolvedConstructExpr>(E)) {
            return CXXUCE->arg_size();
        }

        if (const ExprWithCleanups* EWC = dyn_cast<ExprWithCleanups>(E)) {
            return EWC->getNumObjects();
        }
    }

    return clang_Cursor_getNumArguments(C);
}

int clangsharp_Cursor_getNumAssocs(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const GenericSelectionExpr* GSE = dyn_cast<GenericSelectionExpr>(E)) {
            return GSE->getNumAssocs();
        }
    }

    return -1;
}

int clangsharp_Cursor_getNumAssociatedConstraints(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassTemplatePartialSpecializationDecl* CTPSD = dyn_cast<ClassTemplatePartialSpecializationDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            CTPSD->getAssociatedConstraints(associatedConstraints);
            return associatedConstraints.size();
        }

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            NTTPD->getAssociatedConstraints(associatedConstraints);
            return associatedConstraints.size();
        }

        if (const TemplateDecl* TD = dyn_cast<TemplateDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            TD->getAssociatedConstraints(associatedConstraints);
            return associatedConstraints.size();
        }

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            TTPD->getAssociatedConstraints(associatedConstraints);
            return associatedConstraints.size();
        }

        if (const VarTemplatePartialSpecializationDecl* VTPSD = dyn_cast<VarTemplatePartialSpecializationDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            VTPSD->getAssociatedConstraints(associatedConstraints);
            return associatedConstraints.size();
        }
    }

    return -1;
}

int clangsharp_Cursor_getNumAttrs(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const AttributedStmt* AS = dyn_cast<AttributedStmt>(S)) {
            return AS->getAttrs().size();
        }
    }

    return -1;
}

int clangsharp_Cursor_getNumCaptures(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->captures().size();
        }
    }

    return -1;
}

int clangsharp_Cursor_getNumChildren(CXCursor C) {
    if (clang_isExpression(C.kind) || clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);
        unsigned n = 0;

        for (auto child : S->children()) {
            n++;
        }

        return n;
    }

    return -1;
}

int clangsharp_Cursor_getNumDecls(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXNewExpr* CXXNE = dyn_cast<CXXNewExpr>(E)) {
            return 2;
        }

        if (const FunctionParmPackExpr* FPPE = dyn_cast<FunctionParmPackExpr>(E)) {
            return FPPE->getNumExpansions();
        }

        if (const OverloadExpr* OE = dyn_cast<OverloadExpr>(E)) {
            return OE->getNumDecls();
        }
    }

    return -1;
}

int clangsharp_Cursor_getNumSpecializations(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            return 1;
        }
    }

    return 0;
}

int clangsharp_Cursor_getNumTemplateArguments(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            return CSFSD->getTemplateArgsAsWritten()->getNumTemplateArgs();
        }

        if (const ClassTemplatePartialSpecializationDecl* CTPSD = dyn_cast<ClassTemplatePartialSpecializationDecl>(D)) {
            return CTPSD->getTemplateParameters()->size();
        }

        if (const ClassTemplateSpecializationDecl* CTSD = dyn_cast<ClassTemplateSpecializationDecl>(D)) {
            return CTSD->getTemplateArgs().size();
        }

        if (const TemplateDecl* TD = dyn_cast<TemplateDecl>(D)) {
            return TD->getTemplateParameters()->size();
        }

        if (const VarTemplatePartialSpecializationDecl* VTPSD = dyn_cast<VarTemplatePartialSpecializationDecl>(D)) {
            return VTPSD->getTemplateArgs().size();
        }

        if (const VarTemplateSpecializationDecl* VTSD = dyn_cast<VarTemplateSpecializationDecl>(D)) {
            return VTSD->getTemplateArgs().size();
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDependentScopeMemberExpr* CXXDSME = dyn_cast<CXXDependentScopeMemberExpr>(E)) {
            return CXXDSME->getNumTemplateArgs();
        }

        if (const DeclRefExpr* DRE = dyn_cast<DeclRefExpr>(E)) {
            return DRE->getNumTemplateArgs();
        }

        if (const DependentScopeDeclRefExpr* DSDRE = dyn_cast<DependentScopeDeclRefExpr>(E)) {
            return DSDRE->getNumTemplateArgs();
        }

        if (const MemberExpr* ME = dyn_cast<MemberExpr>(E)) {
            return ME->getNumTemplateArgs();
        }

        if (const OverloadExpr* OE = dyn_cast<OverloadExpr>(E)) {
            return OE->getNumTemplateArgs();
        }

        if (const SizeOfPackExpr* SOPE = dyn_cast<SizeOfPackExpr>(E)) {
            return SOPE->getPartialArguments().size();
        }

        if (const SubstNonTypeTemplateParmPackExpr* SNTTPPE = dyn_cast<SubstNonTypeTemplateParmPackExpr>(E)) {
            return 1;
        }
    }

    return clang_Cursor_getNumTemplateArguments(C);
}

CXCursor clangsharp_Cursor_getOpaqueValue(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const BinaryConditionalOperator* BCO = dyn_cast<BinaryConditionalOperator>(E)) {
            return MakeCXCursor(BCO->getOpaqueValue(), getCursorDecl(C), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXType clangsharp_Cursor_getOriginalType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ParmVarDecl* PVD = dyn_cast<ParmVarDecl>(D)) {
            return MakeCXType(PVD->getOriginalType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CX_OverloadedOperatorKind clangsharp_Cursor_getOverloadedOperatorKind(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXOperatorCallExpr* CXXOCE = dyn_cast<CXXOperatorCallExpr>(E)) {
            return static_cast<CX_OverloadedOperatorKind>(CXXOCE->getOperator());
        }
    }

    return CX_OO_Invalid;
}

int clangsharp_Cursor_getPackLength(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const SizeOfPackExpr* SOPE = dyn_cast<SizeOfPackExpr>(E)) {
            return SOPE->getPackLength();
        }
    }

    return -1;
}

CXCursor clangsharp_Cursor_getParentFunctionOrMethod(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const Decl* PD = dyn_cast_or_null<Decl>(D->getParentFunctionOrMethod())) {
            return MakeCXCursor(PD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getPlaceholderTypeConstraint(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            return MakeCXCursor(NTTPD->getPlaceholderTypeConstraint(), NTTPD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getPreviousDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getPreviousDecl(), getCursorTU(C));
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getPrimaryTemplate(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return MakeCXCursor(FD->getPrimaryTemplate(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getRedeclContext(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const DeclContext* DC = dyn_cast<DeclContext>(D)) {
            return MakeCXCursor(dyn_cast<Decl>(DC->getRedeclContext()), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getReferenced(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const AddrLabelExpr* ALE = dyn_cast<AddrLabelExpr>(E)) {
            return MakeCXCursor(ALE->getLabel(), getCursorTU(C));
        }

        if (const BlockExpr* BE = dyn_cast<BlockExpr>(E)) {
            return MakeCXCursor(BE->getBlockDecl(), getCursorTU(C));
        }

        if (const CXXConstructExpr* CXXCE = dyn_cast<CXXConstructExpr>(E)) {
            return MakeCXCursor(CXXCE->getConstructor(), getCursorTU(C));
        }

        if (const CXXDefaultArgExpr* CXXDAE = dyn_cast<CXXDefaultArgExpr>(E)) {
            return MakeCXCursor(CXXDAE->getParam(), getCursorTU(C));
        }

        if (const CXXDefaultInitExpr* CXXDIE = dyn_cast<CXXDefaultInitExpr>(E)) {
            return MakeCXCursor(CXXDIE->getField(), getCursorTU(C));
        }

        if (const CXXDeleteExpr* CXXDE = dyn_cast<CXXDeleteExpr>(E)) {
            return MakeCXCursor(CXXDE->getOperatorDelete(), getCursorTU(C));
        }

        if (const CXXDependentScopeMemberExpr* CXXDSME = dyn_cast<CXXDependentScopeMemberExpr>(E)) {
            return MakeCXCursor(CXXDSME->getFirstQualifierFoundInScope(), getCursorTU(C));
        }

        if (const CXXUuidofExpr* CXXUE = dyn_cast<CXXUuidofExpr>(E)) {
            return MakeCXCursor(CXXUE->getGuidDecl(), getCursorTU(C));
        }

        if (const FunctionParmPackExpr* FPPE = dyn_cast<FunctionParmPackExpr>(E)) {
            return MakeCXCursor(FPPE->getParameterPack(), getCursorTU(C));
        }

        if (const InitListExpr* ILE = dyn_cast<InitListExpr>(E)) {
            return MakeCXCursor(ILE->getInitializedFieldInUnion(), getCursorTU(C));
        }

        if (const MaterializeTemporaryExpr* MTE = dyn_cast<MaterializeTemporaryExpr>(E)) {
            return MakeCXCursor(MTE->getLifetimeExtendedTemporaryDecl(), getCursorTU(C));
        }

        if (const ObjCArrayLiteral* OCAL = dyn_cast<ObjCArrayLiteral>(E)) {
            return MakeCXCursor(OCAL->getArrayWithObjectsMethod(), getCursorTU(C));
        }

        if (const ObjCBoxedExpr* OCBE = dyn_cast<ObjCBoxedExpr>(E)) {
            return MakeCXCursor(OCBE->getBoxingMethod(), getCursorTU(C));
        }

        if (const ObjCIvarRefExpr* OCIRE = dyn_cast<ObjCIvarRefExpr>(E)) {
            return MakeCXCursor(OCIRE->getDecl(), getCursorTU(C));
        }

        if (const ObjCProtocolExpr* OCPE = dyn_cast<ObjCProtocolExpr>(E)) {
            return MakeCXCursor(OCPE->getProtocol(), getCursorTU(C));
        }

        if (const ObjCSubscriptRefExpr* OCSRE = dyn_cast<ObjCSubscriptRefExpr>(E)) {
            return MakeCXCursor(OCSRE->getAtIndexMethodDecl(), getCursorTU(C));
        }

        if (const OverloadExpr* OE = dyn_cast<OverloadExpr>(E)) {
            return MakeCXCursor(OE->getNamingClass(), getCursorTU(C));
        }

        if (const SubstNonTypeTemplateParmExpr* SNTTPE = dyn_cast<SubstNonTypeTemplateParmExpr>(E)) {
            return MakeCXCursor(SNTTPE->getParameter(), getCursorTU(C));
        }

        if (const SubstNonTypeTemplateParmPackExpr* SNTTPPE = dyn_cast<SubstNonTypeTemplateParmPackExpr>(E)) {
            return MakeCXCursor(SNTTPPE->getParameterPack(), getCursorTU(C));
        }
    }

    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const CXXCatchStmt* CXXCS = dyn_cast<CXXCatchStmt>(S)) {
            return MakeCXCursor(CXXCS->getExceptionDecl(), getCursorTU(C));
        }

        if (const GotoStmt* GS = dyn_cast<GotoStmt>(S)) {
            return MakeCXCursor(GS->getLabel(), getCursorTU(C));
        }

        if (const IndirectGotoStmt* IGS = dyn_cast<IndirectGotoStmt>(S)) {
            return MakeCXCursor(IGS->getConstantTarget(), getCursorTU(C));
        }

        if (const LabelStmt* LS = dyn_cast<LabelStmt>(S)) {
            return MakeCXCursor(LS->getDecl(), getCursorTU(C));
        }

        if (const ObjCAtCatchStmt* OCACS = dyn_cast<ObjCAtCatchStmt>(S)) {
            return MakeCXCursor(OCACS->getCatchParamDecl(), getCursorTU(C));
        }

        if (const ReturnStmt* RS = dyn_cast<ReturnStmt>(S)) {
            return MakeCXCursor(RS->getNRVOCandidate(), getCursorTU(C));
        }
    }

    return clang_getCursorReferenced(C);
}

unsigned clangsharp_Cursor_getRequiresZeroInitialization(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXConstructExpr* CXXCE = dyn_cast<CXXConstructExpr>(E)) {
            return CXXCE->requiresZeroInitialization();
        }
    }

    return 0;
}

int clangsharp_Cursor_getResultIndex(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const GenericSelectionExpr* GSE = dyn_cast<GenericSelectionExpr>(E)) {
            if (!GSE->isResultDependent()) {
                return GSE->getResultIndex();
            }
        }

        if (const PseudoObjectExpr* POE = dyn_cast<PseudoObjectExpr>(E)) {
            return POE->getResultExprIndex();
        }
    }

    return -1;
}

CXType clangsharp_Cursor_getReturnType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return MakeCXType(FD->getReturnType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CXCursor clangsharp_Cursor_getRhsExpr(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXRewrittenBinaryOperator* CRBO = dyn_cast<CXXRewrittenBinaryOperator>(E)) {
            return MakeCXCursor(CRBO->getRHS(), getCursorDecl(C), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

unsigned clangsharp_Cursor_getShouldCopy(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const ObjCIndirectCopyRestoreExpr* OCICRE = dyn_cast<ObjCIndirectCopyRestoreExpr>(E)) {
            return OCICRE->shouldCopy();
        }
    }

    return 0;
}

CXSourceRange clangsharp_Cursor_getSourceRange(CXCursor C) {
    SourceRange R = getCursorSourceRange(C);

    if (R.isInvalid())
        return clang_getNullRange();

    return translateSourceRange(getCursorContext(C), R);
}

CXCursor clangsharp_Cursor_getSpecialization(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            if (i == 0) {
                return MakeCXCursor(CSFSD->getSpecialization(), getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CX_StmtClass clangsharp_Cursor_getStmtClass(CXCursor C) {
    if (clang_isExpression(C.kind) || clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);
        return static_cast<CX_StmtClass>(S->getStmtClass());
    }

    return CX_StmtClass_Invalid;
}

CXString clangsharp_Cursor_getStringLiteralValue(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const StringLiteral* SL = dyn_cast<StringLiteral>(E)) {
            return createDup(SL->getString());
        }
    }

    return createEmpty();
}

CXCursor clangsharp_Cursor_getSubExpr(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const InitListExpr* ILE = dyn_cast<InitListExpr>(E)) {
            return MakeCXCursor(ILE->getArrayFiller(), getCursorDecl(C), getCursorTU(C));
        }

        if (const OpaqueValueExpr* OVE = dyn_cast<OpaqueValueExpr>(E)) {
            return MakeCXCursor(OVE->getSourceExpr(), getCursorDecl(C), getCursorTU(C));
        }

        if (const UnaryExprOrTypeTraitExpr* UETTE = dyn_cast<UnaryExprOrTypeTraitExpr>(E)) {
            if (!UETTE->isArgumentType()) {
                return MakeCXCursor(UETTE->getArgumentExpr(), getCursorDecl(C), getCursorTU(C));
            }
        }

        if (const UnaryOperator* UO = dyn_cast<UnaryOperator>(E)) {
            return MakeCXCursor(UO->getSubExpr(), getCursorDecl(C), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getSubStmt(CXCursor C) {
    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const SwitchStmt* SS = dyn_cast<SwitchStmt>(S)) {
            return MakeCXCursor(SS->getSwitchCaseList(), getCursorDecl(C), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getSubExprAsWritten(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CastExpr* CE = dyn_cast<CastExpr>(E)) {
            return MakeCXCursor(CE->getSubExprAsWritten(), getCursorDecl(C), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getTargetUnionField(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CastExpr* CE = dyn_cast<CastExpr>(E)) {
            if (CE->getCastKind() == CK_ToUnion) {
                return MakeCXCursor(CE->getTargetUnionField(), getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CX_TemplateArgument clangsharp_Cursor_getTemplateArgument(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassTemplateSpecializationDecl* CTSD = dyn_cast<ClassTemplateSpecializationDecl>(D)) {
            if (i < CTSD->getTemplateArgs().size()) {
                const TemplateArgument* TA = &CTSD->getTemplateArgs()[i];
                return MakeCXTemplateArgument(TA, C, getCursorTU(C));
            }
        }

        if (const VarTemplatePartialSpecializationDecl* VTPSD = dyn_cast<VarTemplatePartialSpecializationDecl>(D)) {
            if (i < VTPSD->getTemplateArgs().size()) {
                const TemplateArgument* TA = &VTPSD->getTemplateArgs()[i];
                return MakeCXTemplateArgument(TA, C, getCursorTU(C));
            }
        }

        if (const VarTemplateSpecializationDecl* VTSD = dyn_cast<VarTemplateSpecializationDecl>(D)) {
            if (i < VTSD->getTemplateArgs().size()) {
                const TemplateArgument* TA = &VTSD->getTemplateArgs()[i];
                return MakeCXTemplateArgument(TA, C, getCursorTU(C));
            }
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const SizeOfPackExpr* SOPE = dyn_cast<SizeOfPackExpr>(E)) {
            ArrayRef<TemplateArgument> partialArguments = SOPE->getPartialArguments();

            if (i < partialArguments.size()) {
                const TemplateArgument* TA = &partialArguments[i];
                return MakeCXTemplateArgument(TA, C, getCursorTU(C));
            }
        }

        if (const SubstNonTypeTemplateParmPackExpr* SNTTPPE = dyn_cast<SubstNonTypeTemplateParmPackExpr>(E)) {
            if (i == 0) {
                const TemplateArgument* TA = &SNTTPPE->getArgumentPack();
                return MakeCXTemplateArgument(TA, C, getCursorTU(C));
            }
        }
    }

    return MakeCXTemplateArgument(nullptr, C, getCursorTU(C));
}

CX_TemplateArgumentLoc clangsharp_Cursor_getTemplateArgumentLoc(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            if (i < CSFSD->getTemplateArgsAsWritten()->getNumTemplateArgs()) {
                const TemplateArgumentLoc* TAL = &CSFSD->getTemplateArgsAsWritten()->getTemplateArgs()[i];
                return MakeCXTemplateArgumentLoc(TAL, C, getCursorTU(C));
            }
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            if (i == 0) {
                const TemplateArgumentLoc* TAL = &TTPD->getDefaultArgument();
                return MakeCXTemplateArgumentLoc(TAL, C, getCursorTU(C));
            }
        }
    }

    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDependentScopeMemberExpr* CXXDSME = dyn_cast<CXXDependentScopeMemberExpr>(E)) {
            if (i < CXXDSME->getNumTemplateArgs()) {
                const TemplateArgumentLoc* TAL = &CXXDSME->getTemplateArgs()[i];
                return MakeCXTemplateArgumentLoc(TAL, C, getCursorTU(C));
            }
        }

        if (const DeclRefExpr* DRE = dyn_cast<DeclRefExpr>(E)) {
            if (i < DRE->getNumTemplateArgs()) {
                const TemplateArgumentLoc* TAL = &DRE->getTemplateArgs()[i];
                return MakeCXTemplateArgumentLoc(TAL, C, getCursorTU(C));
            }
        }

        if (const DependentScopeDeclRefExpr* DSDRE = dyn_cast<DependentScopeDeclRefExpr>(E)) {
            if (i < DSDRE->getNumTemplateArgs()) {
                const TemplateArgumentLoc* TAL = &DSDRE->getTemplateArgs()[i];
                return MakeCXTemplateArgumentLoc(TAL, C, getCursorTU(C));
            }
        }

        if (const MemberExpr* ME = dyn_cast<MemberExpr>(E)) {
            if (i < ME->getNumTemplateArgs()) {
                const TemplateArgumentLoc* TAL = &ME->getTemplateArgs()[i];
                return MakeCXTemplateArgumentLoc(TAL, C, getCursorTU(C));
            }
        }

        if (const OverloadExpr* OE = dyn_cast<OverloadExpr>(E)) {
            if (i < OE->getNumTemplateArgs()) {
                const TemplateArgumentLoc* TAL = &OE->getTemplateArgs()[i];
                return MakeCXTemplateArgumentLoc(TAL, C, getCursorTU(C));
            }
        }
    }

    return MakeCXTemplateArgumentLoc(nullptr, C, getCursorTU(C));
}

CXCursor clangsharp_Cursor_getTemplateParameter(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassTemplatePartialSpecializationDecl* CTPSD = dyn_cast<ClassTemplatePartialSpecializationDecl>(D)) {
            TemplateParameterList* templateParameters = CTPSD->getTemplateParameters();

            if (i < templateParameters->size()) {
                return MakeCXCursor(templateParameters->getParam(i), getCursorTU(C));
            }
        }

        if (const TemplateDecl* TD = dyn_cast<TemplateDecl>(D)) {
            TemplateParameterList* templateParameters = TD->getTemplateParameters();

            if (i < templateParameters->size()) {
                return MakeCXCursor(templateParameters->getParam(i), getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getTemplatedDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const TemplateDecl* TD = dyn_cast<TemplateDecl>(D)) {
            return MakeCXCursor(TD->getTemplatedDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getTemplateInstantiationPattern(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<CXXRecordDecl>(D)) {
            return MakeCXCursor(CRD->getTemplateInstantiationPattern(), getCursorTU(C));
        }

        if (const EnumDecl* ED = dyn_cast<EnumDecl>(D)) {
            return MakeCXCursor(ED->getTemplateInstantiationPattern(), getCursorTU(C));
        }

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return MakeCXCursor(FD->getTemplateInstantiationPattern(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CX_TemplateSpecializationKind clangsharp_Cursor_getTemplateSpecializationKind(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXRecordDecl* CRD = dyn_cast<ClassTemplateSpecializationDecl>(D)) {
            return static_cast<CX_TemplateSpecializationKind>(CRD->getTemplateSpecializationKind() + 1);
        }

        if (const EnumDecl* ED = dyn_cast<EnumDecl>(D)) {
            return static_cast<CX_TemplateSpecializationKind>(ED->getTemplateSpecializationKind() + 1);
        }

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return static_cast<CX_TemplateSpecializationKind>(FD->getTemplateSpecializationKind() + 1);
        }
    }

    return CX_TSK_Invalid;
}

int clangsharp_Cursor_getTemplateTypeParmDepth(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            return TTPD->getDepth();
        }
    }

    return -1;
}

int clangsharp_Cursor_getTemplateTypeParmIndex(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            return TTPD->getIndex();
        }
    }

    return -1;
}

CXType clangsharp_Cursor_getThisObjectType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXMethodDecl* CMD = dyn_cast<CXXMethodDecl>(D)) {
            return MakeCXType(CMD->getThisObjectType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CXType clangsharp_Cursor_getThisType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXMethodDecl* CMD = dyn_cast<CXXMethodDecl>(D)) {
            return MakeCXType(CMD->getThisType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CXCursor clangsharp_Cursor_getTrailingRequiresClause(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const DeclaratorDecl* DD = dyn_cast<DeclaratorDecl>(D)) {
            return MakeCXCursor(DD->getTrailingRequiresClause(), DD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getTypedefNameForAnonDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const TagDecl* TD = dyn_cast<TagDecl>(D)) {
            return MakeCXCursor(TD->getTypedefNameForAnonDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXType clangsharp_Cursor_getTypeOperand(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CompoundLiteralExpr* CLE = dyn_cast<CompoundLiteralExpr>(E)) {
            return MakeCXType(CLE->getTypeSourceInfo()->getType(), getCursorTU(C));
        }

        if (const CXXDeleteExpr* CXXDE = dyn_cast<CXXDeleteExpr>(E)) {
            return MakeCXType(CXXDE->getDestroyedType(), getCursorTU(C));
        }

        if (const CXXDependentScopeMemberExpr* CXXDSME = dyn_cast<CXXDependentScopeMemberExpr>(E)) {
            return MakeCXType(CXXDSME->getBaseType(), getCursorTU(C));
        }

        if (const CXXScalarValueInitExpr* CXXSVIE = dyn_cast<CXXScalarValueInitExpr>(E)) {
            return MakeCXType(CXXSVIE->getTypeSourceInfo()->getType(), getCursorTU(C));
        }

        if (const CXXTemporaryObjectExpr* CXXTOE = dyn_cast<CXXTemporaryObjectExpr>(E)) {
            return MakeCXType(CXXTOE->getTypeSourceInfo()->getType(), getCursorTU(C));
        }

        if (const CXXTypeidExpr* CXXTE = dyn_cast<CXXTypeidExpr>(E)) {
            if (CXXTE->isTypeOperand()) {
                return MakeCXType(CXXTE->getTypeOperandSourceInfo()->getType(), getCursorTU(C));
            }
        }

        if (const CXXUnresolvedConstructExpr* CXXUCE = dyn_cast<CXXUnresolvedConstructExpr>(E)) {
            return MakeCXType(CXXUCE->getTypeAsWritten(), getCursorTU(C));
        }

        if (const CXXUuidofExpr* CXXUE = dyn_cast<CXXUuidofExpr>(E)) {
            if (CXXUE->isTypeOperand()) {
                return MakeCXType(CXXUE->getTypeOperandSourceInfo()->getType(), getCursorTU(C));
            }
        }

        if (const ExplicitCastExpr* ECE = dyn_cast<ExplicitCastExpr>(E)) {
            return MakeCXType(ECE->getTypeAsWritten(), getCursorTU(C));
        }

        if (const ObjCEncodeExpr* OCEE = dyn_cast<ObjCEncodeExpr>(E)) {
            return MakeCXType(OCEE->getEncodedType(), getCursorTU(C));
        }

        if (const OffsetOfExpr* OOE = dyn_cast<OffsetOfExpr>(E)) {
            return MakeCXType(OOE->getTypeSourceInfo()->getType(), getCursorTU(C));
        }

        if (const CXXPseudoDestructorExpr* CXXPSDE = dyn_cast<CXXPseudoDestructorExpr>(E)) {
            return MakeCXType(CXXPSDE->getDestroyedType(), getCursorTU(C));
        }

        if (const UnresolvedMemberExpr* UME = dyn_cast<UnresolvedMemberExpr>(E)) {
            return MakeCXType(UME->getBaseType(), getCursorTU(C));
        }
    }

    if (clang_isStatement(C.kind)) {
        const Stmt* S = getCursorStmt(C);

        if (const CXXCatchStmt* CXXCS = dyn_cast<CXXCatchStmt>(S)) {
            return MakeCXType(CXXCS->getCaughtType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

CX_UnaryExprOrTypeTrait clangsharp_Cursor_getUnaryExprOrTypeTraitKind(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const UnaryExprOrTypeTraitExpr* UETTE = dyn_cast<UnaryExprOrTypeTraitExpr>(E)) {
            return static_cast<CX_UnaryExprOrTypeTrait>(UETTE->getKind() + 1);
        }
    }

    return CX_UETT_Invalid;
}

CX_UnaryOperatorKind clangsharp_Cursor_getUnaryOpcode(CXCursor C) {
    if (clang_isExpression(C.kind)) {
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

CXCursor clangsharp_Cursor_getUnderlyingDecl(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const NamedDecl* ND = dyn_cast<NamedDecl>(D)) {
            return MakeCXCursor(ND->getUnderlyingDecl(), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getUninstantiatedDefaultArg(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ParmVarDecl* PVD = dyn_cast<ParmVarDecl>(D)) {
            return MakeCXCursor(PVD->getUninstantiatedDefaultArg(), PVD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getUsedContext(CXCursor C) {
    if (clang_isExpression(C.kind)) {
        const Expr* E = getCursorExpr(C);

        if (const CXXDefaultArgExpr* CDAE = dyn_cast<CXXDefaultArgExpr>(E)) {
            return MakeCXCursor(dyn_cast<Decl>(CDAE->getUsedContext()), getCursorTU(C));
        }

        if (const CXXDefaultInitExpr* CDIE = dyn_cast<CXXDefaultInitExpr>(E)) {
            return MakeCXCursor(dyn_cast<Decl>(CDIE->getUsedContext()), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

int64_t clangsharp_Cursor_getVtblIdx(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const CXXMethodDecl* CMD = dyn_cast<CXXMethodDecl>(D)) {
            if (VTableContextBase::hasVtableSlot(CMD)) {
                VTableContextBase* VTC = getASTUnit(getCursorTU(C))->getASTContext().getVTableContext();

                if (MicrosoftVTableContext* MSVTC = dyn_cast<MicrosoftVTableContext>(VTC)) {
                    MethodVFTableLocation ML = MSVTC->getMethodVFTableLocation(CMD);
                    return ML.Index;
                }

                if (ItaniumVTableContext* IVTC = dyn_cast<ItaniumVTableContext>(VTC)) {
                    return IVTC->getMethodVTableIndex(CMD);
                }
            }
        }
    }

    return -1;
}

CXCursor clangsharp_TemplateArgument_getAsDecl(CX_TemplateArgument T) {
    if (T.kind == CXTemplateArgumentKind_Declaration) {
        return MakeCXCursor(T.value->getAsDecl(), T.tu);
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_TemplateArgument_getAsExpr(CX_TemplateArgument T) {
    if (T.kind == CXTemplateArgumentKind_Expression) {
        const Decl* D = nullptr;

        if (T.xdata == 1) {
            D = getCursorDecl(T.parentCursor);
        }
        else {
            CXCursor C = clang_getTypeDeclaration(T.parentType);
            D = getCursorDecl(C);
        }

        return MakeCXCursor(T.value->getAsExpr(), D, T.tu);
    }

    return clang_getNullCursor();
}

int64_t clangsharp_TemplateArgument_getAsIntegral(CX_TemplateArgument T) {
    if (T.kind == CXTemplateArgumentKind_Integral) {
        return T.value->getAsIntegral().getSExtValue();
    }

    return -1;
}

CX_TemplateName clangsharp_TemplateArgument_getAsTemplate(CX_TemplateArgument T) {
    if (T.kind == CXTemplateArgumentKind_Template) {
        TemplateName TN = T.value->getAsTemplate();
        return { static_cast<CX_TemplateNameKind>(TN.getKind() + 1), TN.getAsVoidPointer() };
    }

    return { };
}

CX_TemplateName clangsharp_TemplateArgument_getAsTemplateOrTemplatePattern(CX_TemplateArgument T) {
    if ((T.kind == CXTemplateArgumentKind_Template) || (T.kind == CXTemplateArgumentKind_TemplateExpansion)) {
        TemplateName TN = T.value->getAsTemplateOrTemplatePattern();
        return { static_cast<CX_TemplateNameKind>(TN.getKind() + 1), TN.getAsVoidPointer() };
    }

    return { };
}

CXType clangsharp_TemplateArgument_getAsType(CX_TemplateArgument T) {
    if (T.kind == CXTemplateArgumentKind_Type) {
        return MakeCXType(T.value->getAsType(), T.tu);
    }

    return MakeCXType(QualType(), T.tu);
}

CX_TemplateArgumentDependence clangsharp_TemplateArgument_getDependence(CX_TemplateArgument T) {
    if (T.value) {
        return static_cast<CX_TemplateArgumentDependence>(T.value->getDependence());
    }

    return CX_TAD_None;
}

CXType clangsharp_TemplateArgument_getIntegralType(CX_TemplateArgument T) {
    if (T.kind == CXTemplateArgumentKind_Integral) {
        return MakeCXType(T.value->getIntegralType(), T.tu);
    }

    return MakeCXType(QualType(), T.tu);
}

CXType clangsharp_TemplateArgument_getNonTypeTemplateArgumentType(CX_TemplateArgument T) {
    if (T.value) {
        return MakeCXType(T.value->getNonTypeTemplateArgumentType(), T.tu);
    }

    return MakeCXType(QualType(), T.tu);
}

CXType clangsharp_TemplateArgument_getNullPtrType(CX_TemplateArgument T) {
    if (T.kind == CXTemplateArgumentKind_NullPtr) {
        return MakeCXType(T.value->getNullPtrType(), T.tu);
    }

    return MakeCXType(QualType(), T.tu);
}

CXType clangsharp_TemplateArgument_getParamTypeForDecl(CX_TemplateArgument T) {
    if (T.kind == CXTemplateArgumentKind_Declaration) {
        return MakeCXType(T.value->getParamTypeForDecl(), T.tu);
    }

    return MakeCXType(QualType(), T.tu);
}

CX_TemplateArgument clangsharp_TemplateArgumentLoc_getArgument(CX_TemplateArgumentLoc T) {
    if (T.xdata != 0) {
        const TemplateArgument* TA = &T.value->getArgument();
        return MakeCXTemplateArgument(TA, T.parentCursor, T.tu);
    }

    return MakeCXTemplateArgument(nullptr, T.parentCursor, T.tu);
}

CXSourceLocation clangsharp_TemplateArgumentLoc_getLocation(CX_TemplateArgumentLoc T) {
    if (T.value) {
        SourceLocation SLoc = T.value->getLocation();
        return translateSourceLocation(getASTUnit(T.tu)->getASTContext(), SLoc);
    }

    return clang_getNullLocation();
}

CXCursor clangsharp_TemplateArgumentLoc_getSourceDeclExpression(CX_TemplateArgumentLoc T) {
    if (T.value) {
        if (T.value->getArgument().getKind() == TemplateArgument::ArgKind::Declaration) {
            const Expr* E = T.value->getSourceDeclExpression();
            return MakeCXCursor(E, getCursorDecl(T.parentCursor), T.tu);
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_TemplateArgumentLoc_getSourceExpression(CX_TemplateArgumentLoc T) {
    if (T.value) {
        if (T.value->getArgument().getKind() == TemplateArgument::ArgKind::Expression) {
            const Expr* E = T.value->getSourceExpression();
            return MakeCXCursor(E, getCursorDecl(T.parentCursor), T.tu);
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_TemplateArgumentLoc_getSourceIntegralExpression(CX_TemplateArgumentLoc T) {
    if (T.value) {
        if (T.value->getArgument().getKind() == TemplateArgument::ArgKind::Integral) {
            const Expr* E = T.value->getSourceIntegralExpression();
            return MakeCXCursor(E, getCursorDecl(T.parentCursor), T.tu);
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_TemplateArgumentLoc_getSourceNullPtrExpression(CX_TemplateArgumentLoc T) {
    if (T.value) {
        if (T.value->getArgument().getKind() == TemplateArgument::ArgKind::NullPtr) {
            const Expr* E = T.value->getSourceNullPtrExpression();
            return MakeCXCursor(E, getCursorDecl(T.parentCursor), T.tu);
        }
    }

    return clang_getNullCursor();
}

CXSourceRange clangsharp_TemplateArgumentLoc_getSourceRange(CX_TemplateArgumentLoc T) {
    if (!T.value) {
        return clang_getNullRange();
    }

    SourceRange R = T.value->getSourceRange();

    if (R.isInvalid()) {
        return clang_getNullRange();
    }

    return translateSourceRange(getCursorContext(T.parentCursor), R);
}

CXType clangsharp_Type_desugar(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

#define ABSTRACT_TYPE(Class, Parent)
#define TYPE(Class, Parent) \
     if (const Class##Type* Ty = dyn_cast<Class##Type>(TP)) { \
         return MakeCXType(Ty->desugar(), GetTypeTU(CT)); \
     }
#include "clang/AST/TypeNodes.inc"

    return MakeCXType(QualType(), GetTypeTU(CT));
}

CXCursor clangsharp_Type_getAddrSpaceExpr(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DependentAddressSpaceType* DAST = dyn_cast<DependentAddressSpaceType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(DAST->getAddrSpaceExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    return clang_getNullCursor();
}

CXType clangsharp_Type_getAdjustedType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const AdjustedType* AT = dyn_cast<AdjustedType>(TP)) {
        return MakeCXType(AT->getAdjustedType(), GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}

CX_AttrKind clangsharp_Type_getAttrKind(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const AttributedType* AT = dyn_cast<AttributedType>(TP)) {
        return static_cast<CX_AttrKind>(AT->getAttrKind() + 1);
    }

    return CX_AttrKind_Invalid;
}

CXType clangsharp_Type_getBaseType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const UnaryTransformType* UTT = dyn_cast<UnaryTransformType>(TP)) {
        return MakeCXType(UTT->getBaseType(), GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}

CXCursor clangsharp_Type_getColumnExpr(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DependentSizedMatrixType* DSMT = dyn_cast<DependentSizedMatrixType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(DSMT->getColumnExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    return clang_getNullCursor();
}

CXType clangsharp_Type_getDecayedType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DecayedType* DT = dyn_cast<DecayedType>(TP)) {
        return MakeCXType(DT->getDecayedType(), GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}

CXCursor clangsharp_Type_getDeclaration(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const TemplateTypeParmType* TTPT = dyn_cast<TemplateTypeParmType>(TP)) {
        return MakeCXCursor(TTPT->getDecl(), GetTypeTU(CT));
    }

    if (const UnresolvedUsingType* UUT = dyn_cast<UnresolvedUsingType>(TP)) {
        return MakeCXCursor(UUT->getDecl(), GetTypeTU(CT));
    }

    return clang_getTypeDeclaration(CT);
}

CXType clangsharp_Type_getDeducedType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DeducedType* DT = dyn_cast<DeducedType>(TP)) {
        return MakeCXType(DT->getDeducedType(), GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}

int clangsharp_Type_getDepth(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const TemplateTypeParmType* TTPT = dyn_cast<TemplateTypeParmType>(TP)) {
        return TTPT->getDepth();
    }

    return -1;
}

CXType clangsharp_Type_getElementType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DependentSizedExtVectorType* DSEVT = dyn_cast<DependentSizedExtVectorType>(TP)) {
        return MakeCXType(DSEVT->getElementType(), GetTypeTU(CT));
    }

    if (const DependentVectorType* DVT = dyn_cast<DependentVectorType>(TP)) {
        return MakeCXType(DVT->getElementType(), GetTypeTU(CT));
    }

    if (const MatrixType* MT = dyn_cast<MatrixType>(TP)) {
        return MakeCXType(MT->getElementType(), GetTypeTU(CT));
    }

    if (const PipeType* PT = dyn_cast<PipeType>(TP)) {
        return MakeCXType(PT->getElementType(), GetTypeTU(CT));
    }

    return clang_getElementType(CT);
}

CXType clangsharp_Type_getEquivalentType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const AttributedType* AT = dyn_cast<AttributedType>(TP)) {
        return MakeCXType(AT->getEquivalentType(), GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}

int clangsharp_Type_getIndex(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const TemplateTypeParmType* TTPT = dyn_cast<TemplateTypeParmType>(TP)) {
        return TTPT->getIndex();
    }

    return -1;
}

CXType clangsharp_Type_getInjectedSpecializationType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const InjectedClassNameType* ICNT = dyn_cast<InjectedClassNameType>(TP)) {
        return MakeCXType(ICNT->getInjectedSpecializationType(), GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}

CXType clangsharp_Type_getInjectedTST(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const InjectedClassNameType* ICNT = dyn_cast<InjectedClassNameType>(TP)) {
        QualType QT = QualType(ICNT->getInjectedTST(), 0);
        return MakeCXType(QT, GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}

unsigned clangsharp_Type_getIsSigned(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DependentExtIntType* DEIT = dyn_cast<DependentExtIntType>(TP)) {
        return DEIT->isSigned();
    }

    if (const ExtIntType* EIT = dyn_cast<ExtIntType>(TP)) {
        return EIT->isSigned();
    }

    return 0;
}

unsigned clangsharp_Type_getIsSugared(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

#define ABSTRACT_TYPE(Class, Parent)
#define TYPE(Class, Parent) \
     if (const Class##Type* Ty = dyn_cast<Class##Type>(TP)) { \
         return Ty->isSugared(); \
     }
#include "clang/AST/TypeNodes.inc"

    return 0;
}

unsigned clangsharp_Type_getIsTypeAlias(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const TemplateSpecializationType* TST = dyn_cast<TemplateSpecializationType>(TP)) {
        return TST->isTypeAlias();
    }

    return 0;
}

unsigned clangsharp_Type_getIsUnsigned(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DependentExtIntType* DEIT = dyn_cast<DependentExtIntType>(TP)) {
        return DEIT->isUnsigned();
    }

    if (const ExtIntType* EIT = dyn_cast<ExtIntType>(TP)) {
        return EIT->isUnsigned();
    }

    return 0;
}

CXType clangsharp_Type_getModifiedType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const MacroQualifiedType* MQT = dyn_cast<MacroQualifiedType>(TP)) {
        return MakeCXType(MQT->getModifiedType(), GetTypeTU(CT));
    }

    return clang_Type_getModifiedType(CT);
}

int clangsharp_Type_getNumBits(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const ExtIntType* EIT = dyn_cast<ExtIntType>(TP)) {
        return EIT->getNumBits();
    }

    return -1;
}

CXCursor clangsharp_Type_getNumBitsExpr(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DependentExtIntType* DEIT = dyn_cast<DependentExtIntType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(DEIT->getNumBitsExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    return clang_getNullCursor();
}

int clangsharp_Type_getNumColumns(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const ConstantMatrixType* CMT = dyn_cast<ConstantMatrixType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return CMT->getNumColumns();
    }

    return -1;
}

int clangsharp_Type_getNumElementsFlattened(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const ConstantMatrixType* CMT = dyn_cast<ConstantMatrixType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return CMT->getNumElementsFlattened();
    }

    return -1;
}

int clangsharp_Type_getNumRows(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const ConstantMatrixType* CMT = dyn_cast<ConstantMatrixType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return CMT->getNumRows();
    }

    return -1;
}

CXType clangsharp_Type_getOriginalType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const AdjustedType* AT = dyn_cast<AdjustedType>(TP)) {
        return MakeCXType(AT->getOriginalType(), GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}

CXCursor clangsharp_Type_getOwnedTagDecl(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const ElaboratedType* ET = dyn_cast<ElaboratedType>(TP)) {
        return MakeCXCursor(ET->getOwnedTagDecl(), GetTypeTU(CT));
    }

    return clang_getNullCursor();
}

CXType clangsharp_Type_getPointeeType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (TP) {
        return MakeCXType(TP->getPointeeType(), GetTypeTU(CT));
    }

    return clang_getPointeeType(CT);
}

CXCursor clangsharp_Type_getRowExpr(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DependentSizedMatrixType* DSMT = dyn_cast<DependentSizedMatrixType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(DSMT->getRowExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Type_getSizeExpr(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const ConstantArrayType* CAT = dyn_cast<ConstantArrayType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(CAT->getSizeExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    if (const DependentSizedArrayType* DSAT = dyn_cast<DependentSizedArrayType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(DSAT->getSizeExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    if (const DependentSizedExtVectorType* DSEVT = dyn_cast<DependentSizedExtVectorType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(DSEVT->getSizeExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    if (const DependentVectorType* DVT = dyn_cast<DependentVectorType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(DVT->getSizeExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    if (const VariableArrayType* VAT = dyn_cast<VariableArrayType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(VAT->getSizeExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    return clang_getNullCursor();
}

CX_TemplateArgument clangsharp_Type_getTemplateArgument(CXType CT, unsigned i) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const AutoType* AT = dyn_cast<AutoType>(TP)) {
        if (i < AT->getNumArgs()) {
            return MakeCXTemplateArgument(&AT->getArg(i), CT, GetTypeTU(CT));
        }
    }

    if (const DependentTemplateSpecializationType* DTST = dyn_cast<DependentTemplateSpecializationType>(TP)) {
        if (i < DTST->getNumArgs()) {
            return MakeCXTemplateArgument(&DTST->getArg(i), CT, GetTypeTU(CT));
        }
    }

    if (const TemplateSpecializationType* TST = dyn_cast<TemplateSpecializationType>(TP)) {
        if (i < TST->getNumArgs()) {
            return MakeCXTemplateArgument(&TST->getArg(i), CT, GetTypeTU(CT));
        }
    }

    return MakeCXTemplateArgument(nullptr, CT, GetTypeTU(CT));
}

CX_TypeClass clangsharp_Type_getTypeClass(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (TP != nullptr) {
        return static_cast<CX_TypeClass>(TP->getTypeClass() + 1);
    }

    return CX_TypeClass_Invalid;
}

CXCursor clangsharp_Type_getUnderlyingExpr(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DecltypeType* DT = dyn_cast<DecltypeType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(DT->getUnderlyingExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    if (const TypeOfExprType* TOET = dyn_cast<TypeOfExprType>(TP)) {
        CXCursor C = clang_getTypeDeclaration(CT);
        return MakeCXCursor(TOET->getUnderlyingExpr(), getCursorDecl(C), GetTypeTU(CT));
    }

    return clang_getNullCursor();
}

CXType clangsharp_Type_getUnderlyingType(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (const DecltypeType* DT = dyn_cast<DecltypeType>(TP)) {
        return MakeCXType(DT->getUnderlyingType(), GetTypeTU(CT));
    }

    if (const MacroQualifiedType* MQT = dyn_cast<MacroQualifiedType>(TP)) {
        return MakeCXType(MQT->getUnderlyingType(), GetTypeTU(CT));
    }

    if (const TypeOfType* TOT = dyn_cast<TypeOfType>(TP)) {
        return MakeCXType(TOT->getUnderlyingType(), GetTypeTU(CT));
    }

    if (const UnaryTransformType* UTT = dyn_cast<UnaryTransformType>(TP)) {
        return MakeCXType(UTT->getUnderlyingType(), GetTypeTU(CT));
    }

    return MakeCXType(QualType(), GetTypeTU(CT));
}
