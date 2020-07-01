// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

#include "ClangSharp.h"
#include "CXCursor.h"
#include "CXLoadedDiagnostic.h"
#include "CXSourceLocation.h"
#include "CXString.h"
#include "CXTranslationUnit.h"
#include "CXType.h"

#include <clang/Basic/SourceManager.h>

using namespace clang;
using namespace clang::cxcursor;
using namespace clang::cxloc;
using namespace clang::cxstring;
using namespace clang::cxtu;
using namespace clang::cxtype;

CXCursor clangsharp_Cursor_getAssociatedConstraint(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassTemplatePartialSpecializationDecl* CTPSD = dyn_cast<ClassTemplatePartialSpecializationDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            CTPSD->getAssociatedConstraints(associatedConstraints);
            return MakeCXCursor(associatedConstraints[i], CTPSD, getCursorTU(C));
        }

        if (const NonTypeTemplateParmDecl* NTTPD = dyn_cast<NonTypeTemplateParmDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            NTTPD->getAssociatedConstraints(associatedConstraints);
            return MakeCXCursor(associatedConstraints[i], NTTPD, getCursorTU(C));
        }

        if (const TemplateDecl* TD = dyn_cast<TemplateDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            TD->getAssociatedConstraints(associatedConstraints);
            return MakeCXCursor(associatedConstraints[i], TD, getCursorTU(C));
        }

        if (const TemplateTypeParmDecl* TTPD = dyn_cast<TemplateTypeParmDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            TTPD->getAssociatedConstraints(associatedConstraints);
            return MakeCXCursor(associatedConstraints[i], TTPD, getCursorTU(C));
        }

        if (const VarTemplatePartialSpecializationDecl* VTPSD = dyn_cast<VarTemplatePartialSpecializationDecl>(D)) {
            SmallVector<const Expr*, 32> associatedConstraints;
            VTPSD->getAssociatedConstraints(associatedConstraints);
            return MakeCXCursor(associatedConstraints[i], VTPSD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getArgument(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return MakeCXCursor(BD->parameters()[i], getCursorTU(C));
        }

        if (const CapturedDecl* CD = dyn_cast<CapturedDecl>(D)) {
            return MakeCXCursor(CD->parameters()[i], getCursorTU(C));
        }
    }

    return clang_Cursor_getArgument(C, i);
}

CXCursor clangsharp_Cursor_getAsFunction(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getAsFunction(), getCursorTU(C));
    }

    return clang_getNullCursor();
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
            return MakeCXCursor(BD->captures()[i].getCopyExpr(), BD, getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

unsigned clangsharp_Cursor_getCaptureHasCopyExpr(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->captures()[i].hasCopyExpr();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCaptureIsByRef(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->captures()[i].isByRef();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCaptureIsEscapingByRef(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->captures()[i].isEscapingByref();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCaptureIsNested(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->captures()[i].isNested();
        }
    }

    return 0;
}

unsigned clangsharp_Cursor_getCaptureIsNonEscapingByRef(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->captures()[i].isNonEscapingByref();
        }
    }

    return 0;
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

unsigned clangSharp_Cursor_getCapturesVariable(CXCursor C, CXCursor V) {

    const BlockDecl* BD = dyn_cast<BlockDecl>(getCursorDecl(C));
    const VarDecl* VD = dyn_cast<VarDecl>(getCursorDecl(V));

    if (!BD || !VD) {
        return 0;
    }

    return BD->capturesVariable(VD);
}

CXCursor clangsharp_Cursor_getCaptureVariable(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return MakeCXCursor(BD->captures()[i].getVariable(), getCursorTU(C));
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

unsigned clangsharp_Cursor_getConstructsVirtualBase(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConstructorUsingShadowDecl* CUSD = dyn_cast<ConstructorUsingShadowDecl>(D)) {
            return CUSD->constructsVirtualBase();
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

CXType clangsharp_Cursor_getDeclaredReturnType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return MakeCXType(FD->getDeclaredReturnType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
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

CXType clangsharp_Cursor_getEnumDeclPromotionType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumDecl* ED = dyn_cast<EnumDecl>(D)) {
            return MakeCXType(ED->getPromotionType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
}

int clangsharp_Cursor_getFieldIndex(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FieldDecl* FD = dyn_cast<FieldDecl>(D)) {
            FD->getFieldIndex();
        }
    }

    return -1;
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
            PVD->getFunctionScopeDepth();
        }
    }

    return -1;
}

int clangsharp_Cursor_getFunctionScopeIndex(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ParmVarDecl* PVD = dyn_cast<ParmVarDecl>(D)) {
            PVD->getFunctionScopeIndex();
        }
    }

    return -1;
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

unsigned clangsharp_Cursor_getHasExplicitTemplateArgs(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            return CSFSD->hasExplicitTemplateArgs();
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

unsigned clangSharp_Cursor_getIsConversionFromLambda(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const BlockDecl* BD = dyn_cast<BlockDecl>(D)) {
            return BD->isConversionFromLambda();
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

unsigned clangSharp_Cursor_getIsDeprecated(CXCursor C) {
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

unsigned clangsharp_Cursor_getIsGlobal(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isGlobal();
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

unsigned clangSharp_Cursor_getIsNegative(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return ECD->getInitVal().isNegative();
        }
    }

    return 0;
}

unsigned clangSharp_Cursor_getIsNonNegative(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return ECD->getInitVal().isNonNegative();
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

unsigned clangSharp_Cursor_getIsNothrow(CXCursor C) {
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

unsigned clangsharp_Cursor_getIsPure(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return FD->isPure();
        }
    }

    return 0;
}

unsigned clangSharp_Cursor_getIsSigned(CXCursor C) {
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

unsigned clangSharp_Cursor_getIsStrictlyPositive(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const EnumConstantDecl* ECD = dyn_cast<EnumConstantDecl>(D)) {
            return ECD->getInitVal().isStrictlyPositive();
        }
    }

    return 0;
}

unsigned clangSharp_Cursor_getIsTemplated(CXCursor C) {
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

unsigned clangsharp_Cursor_getIsTransparentTag(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const TypedefNameDecl* TND = dyn_cast<TypedefNameDecl>(D)) {
            return TND->isTransparentTag();
        }
    }

    return 0;
}

unsigned clangSharp_Cursor_getIsTypeConcept(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ConceptDecl* CTD = dyn_cast<ConceptDecl>(D)) {
            return CTD->isTypeConcept();
        }
    }

    return 0;
}

unsigned clangSharp_Cursor_getIsUnavailable(CXCursor C) {
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

unsigned clangSharp_Cursor_getIsUnsigned(CXCursor C) {
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

unsigned clangSharp_Cursor_getIsVariadic(CXCursor C) {
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

unsigned clangSharp_Cursor_getMaxAlignment(CXCursor C) {
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

CXCursor clangsharp_Cursor_getNextDeclInContext(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(D->getNextDeclInContext(), getCursorTU(C));
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

    return clang_Cursor_getNumArguments(C);
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

        if (const ClassTemplatePartialSpecializationDecl* CTPSD = dyn_cast<ClassTemplatePartialSpecializationDecl>(D)) {
            return CTPSD->getTemplateParameters()->size();
        }

        if (const TemplateDecl* TD = dyn_cast<TemplateDecl>(D)) {
            return TD->getTemplateParameters()->size();
        }
    }

    return clang_Cursor_getNumTemplateArguments(C);
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

CXCursor clangsharp_Cursor_getParentFunctionOrMethod(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);
        return MakeCXCursor(dyn_cast<Decl>(D->getParentFunctionOrMethod()), getCursorTU(C));
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

CXType clangsharp_Cursor_getReturnType(CXCursor C) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const FunctionDecl* FD = dyn_cast<FunctionDecl>(D)) {
            return MakeCXType(FD->getReturnType(), getCursorTU(C));
        }
    }

    return MakeCXType(QualType(), getCursorTU(C));
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

CXCursor clangsharp_Cursor_getTemplateArgument(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassTemplatePartialSpecializationDecl* CTPSD = dyn_cast<ClassTemplatePartialSpecializationDecl>(D)) {
            return MakeCXCursor(CTPSD->getTemplateParameters()->getParam(i), getCursorTU(C));
        }

        if (const TemplateDecl* TD = dyn_cast<TemplateDecl>(D)) {
            return MakeCXCursor(TD->getTemplateParameters()->getParam(i), getCursorTU(C));
        }
    }

    return clang_getNullCursor();
}

CXSourceLocation clangsharp_Cursor_getTemplateArgumentLocLocation(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            SourceLocation SLoc = CSFSD->getTemplateArgsAsWritten()->arguments()[i].getLocation();
            return translateSourceLocation(getASTUnit(getCursorTU(C))->getASTContext(), SLoc);
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            if (i == 0) {
                SourceLocation SLoc = TTPD->getDefaultArgument().getLocation();
                return translateSourceLocation(getASTUnit(getCursorTU(C))->getASTContext(), SLoc);
            }
        }
    }

    return clang_getNullLocation();
}

CXCursor clangsharp_Cursor_getTemplateArgumentLocSourceDeclExpression(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            const Expr* E = CSFSD->getTemplateArgsAsWritten()->arguments()[i].getSourceDeclExpression();
            return MakeCXCursor(E, CSFSD, getCursorTU(C));
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            if (i == 0) {
                const Expr* E = TTPD->getDefaultArgument().getSourceDeclExpression();
                return MakeCXCursor(E, TTPD, getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getTemplateArgumentLocSourceExpression(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            const Expr* E = CSFSD->getTemplateArgsAsWritten()->arguments()[i].getSourceDeclExpression();
            return MakeCXCursor(E, CSFSD, getCursorTU(C));
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            if (i == 0) {
                const Expr* E = TTPD->getDefaultArgument().getSourceExpression();
                return MakeCXCursor(E, TTPD, getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getTemplateArgumentLocSourceIntegralExpression(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            const Expr* E = CSFSD->getTemplateArgsAsWritten()->arguments()[i].getSourceDeclExpression();
            return MakeCXCursor(E, CSFSD, getCursorTU(C));
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            if (i == 0) {
                const Expr* E = TTPD->getDefaultArgument().getSourceIntegralExpression();
                return MakeCXCursor(E, TTPD, getCursorTU(C));
            }
        }
    }

    return clang_getNullCursor();
}

CXCursor clangsharp_Cursor_getTemplateArgumentLocSourceNullPtrExpression(CXCursor C, unsigned i) {
    if (clang_isDeclaration(C.kind)) {
        const Decl* D = getCursorDecl(C);

        if (const ClassScopeFunctionSpecializationDecl* CSFSD = dyn_cast<ClassScopeFunctionSpecializationDecl>(D)) {
            const Expr* E = CSFSD->getTemplateArgsAsWritten()->arguments()[i].getSourceDeclExpression();
            return MakeCXCursor(E, CSFSD, getCursorTU(C));
        }

        if (const TemplateTemplateParmDecl* TTPD = dyn_cast<TemplateTemplateParmDecl>(D)) {
            if (i == 0) {
                const Expr* E = TTPD->getDefaultArgument().getSourceNullPtrExpression();
                return MakeCXCursor(E, TTPD, getCursorTU(C));
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

CX_TypeClass clangsharp_Type_getTypeClass(CXType CT) {
    QualType T = GetQualType(CT);
    const Type* TP = T.getTypePtrOrNull();

    if (TP != nullptr)
    {
        return static_cast<CX_TypeClass>(TP->getTypeClass() + 1);
    }

    return CX_TypeClass_Invalid;
}
