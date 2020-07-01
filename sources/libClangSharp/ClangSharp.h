// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

#ifndef LIBCLANGSHARP_CLANGSHARP_H
#define LIBCLANGSHARP_CLANGSHARP_H

#include <clang/Basic/Specifiers.h>
#include <clang-c/Index.h>

#ifdef __cplusplus
#define EXTERN_C extern "C"
#else
#define EXTERN_C
#endif

#ifdef _MSC_VER
#ifdef CLANGSHARP_LINKAGE
#define CLANGSHARP_LINKAGE EXTERN_C __declspec(dllexport)
#else
#define CLANGSHARP_LINKAGE EXTERN_C __declspec(dllimport)
#endif
#else
#define CLANGSHARP_LINKAGE EXTERN_C
#endif

enum CX_AttrKind {
    CX_AttrKind_Invalid,
#define ATTR(X) CX_AttrKind_##X,
#define ATTR_RANGE(CLASS, FIRST_NAME, LAST_NAME) CX_AttrKind_First##CLASS = CX_AttrKind_##FIRST_NAME, CX_AttrKind_Last##CLASS = CX_AttrKind_##LAST_NAME,
#include <clang/Basic/AttrList.inc>
};

enum CX_BinaryOperatorKind {
    CX_BO_Invalid,
#define BINARY_OPERATION(Name, Spelling) CX_BO_##Name,
#include <clang/AST/OperationKinds.def>
};

enum CX_CastKind {
    CX_CK_Invalid,
#define CAST_OPERATION(Name) CX_CK_##Name,
#include <clang/AST/OperationKinds.def>
};

enum CX_DeclKind {
    CX_DeclKind_Invalid,
#define DECL(DERIVED, BASE) CX_DeclKind_##DERIVED,
#define DECL_RANGE(BASE, START, END) CX_DeclKind_First##BASE = CX_DeclKind_##START, CX_DeclKind_Last##BASE = CX_DeclKind_##END,
#define LAST_DECL_RANGE(BASE, START, END) CX_DeclKind_First##BASE = CX_DeclKind_##START, CX_DeclKind_Last##BASE = CX_DeclKind_##END
#define ABSTRACT_DECL(DECL)
#include <clang/AST/DeclNodes.inc>
};

enum CX_TemplateSpecializationKind {
    CX_TSK_Invalid,
    CX_TSK_Undeclared = clang::TSK_Undeclared,
    CX_TSK_ImplicitInstantiation = clang::TSK_ImplicitInstantiation,
    CX_TSK_ExplicitSpecialization = clang::TSK_ExplicitSpecialization,
    CX_TSK_ExplicitInstantiationDeclaration = clang::TSK_ExplicitInstantiationDeclaration,
    CX_TSK_ExplicitInstantiationDefinition = clang::TSK_ExplicitInstantiationDefinition,
};

enum CX_StmtClass {
    CX_StmtClass_Invalid,
#define STMT(CLASS, PARENT) CX_StmtClass_##CLASS,
#define STMT_RANGE(BASE, FIRST, LAST) CX_StmtClass_First##BASE = CX_StmtClass_##FIRST, CX_StmtClass_Last##BASE = CX_StmtClass_##LAST,
#define LAST_STMT_RANGE(BASE, FIRST, LAST) CX_StmtClass_First##BASE = CX_StmtClass_##FIRST, CX_StmtClass_Last##BASE = CX_StmtClass_##LAST
#define ABSTRACT_STMT(STMT)
#include <clang/AST/StmtNodes.inc>
};

enum CX_TypeClass {
    CX_TypeClass_Invalid,
#define TYPE(Class, Base) CX_TypeClass_##Class,
#define LAST_TYPE(Class) CX_TypeClass_TypeLast = CX_TypeClass_##Class,
#define ABSTRACT_TYPE(Class, Base)
#include <clang/AST/TypeNodes.inc>
    CX_TypeClass_TagFirst = CX_TypeClass_Record, CX_TypeClass_TagLast = CX_TypeClass_Enum
};

enum CX_UnaryOperatorKind {
    CX_UO_Invalid,
#define UNARY_OPERATION(Name, Spelling) CX_UO_##Name,
#include <clang/AST/OperationKinds.def>
};

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAssociatedConstraint(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getArgument(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAsFunction(CXCursor C);

CLANGSHARP_LINKAGE CX_AttrKind clangsharp_Cursor_getAttrKind(CXCursor C);

CLANGSHARP_LINKAGE CX_BinaryOperatorKind clangsharp_Cursor_getBinaryOpcode(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getBinaryOpcodeSpelling(CX_BinaryOperatorKind Op);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBinding(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBitWidth(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBlockManglingContextDecl(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getBlockManglingNumber(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getBlockMissingReturnType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBody(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getCallResultType(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCanAvoidCopyToHeap(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCaptureCopyExpr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureHasCopyExpr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureIsByRef(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureIsEscapingByRef(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureIsNested(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureIsNonEscapingByRef(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCapturesCXXThis(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getCapturesVariable(CXCursor C, CXCursor V);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCaptureVariable(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_CastKind clangsharp_Cursor_getCastKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getConstraintExpr(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getConstructedBaseClass(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getConstructedBaseClassShadowDecl(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getConstructsVirtualBase(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getContextParam(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getContextParamPosition(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getDeclaredReturnType(CXCursor C);

CLANGSHARP_LINKAGE CX_DeclKind clangsharp_Cursor_getDeclKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDecomposedDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getDefaultArgType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDependentLambdaCallOperator(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDescribedClassTemplate(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDescribedTemplate(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDestructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getDoesNotEscape(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getEnumDeclPromotionType(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getFieldIndex(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getFriendDecl(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getFunctionScopeDepth(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getFunctionScopeIndex(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasBody(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasExplicitTemplateArgs(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasExternalStorage(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasGlobalStorage(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasImplicitReturnZero(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasInheritedDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasInit(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasLocalStorage(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasPlaceholderTypeConstraint(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getHoldingVar(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInClassInitializer(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInitExpr(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInstantiatedFromMember(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsAnonymousStructOrUnion(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsConversionFromLambda(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsDefined(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsDeprecated(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsExpandedParameterPack(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsExternC(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsGlobal(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsLocalVarDecl(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsLocalVarDeclOrParm(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsMemberSpecialization(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsNegative(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsNonNegative(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsNoReturn(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsNothrow(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsOverloadedOperator(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsPackExpansion(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsParameterPack(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsPure(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsSigned(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsStatic(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsStaticDataMember(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsStrictlyPositive(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsTemplated(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsThisDeclarationADefinition(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsTransparentType(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsTypeConcept(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsUnavailable(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsUnnamedBitfield(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsUnsigned(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsUnsupportedFriend(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getIsVariadic(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaCallOperator(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaContextDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaStaticInvoker(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangSharp_Cursor_getMaxAlignment(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getMostRecentDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getMostRecentNonInjectedDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNextDeclInContext(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNominatedBaseClass(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNominatedBaseClassShadowDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNonClosureContext(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumAssociatedConstraints(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumArguments(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumCaptures(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumSpecializations(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumTemplateArguments(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getOriginalType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getParentFunctionOrMethod(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPlaceholderTypeConstraint(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPreviousDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPrimaryTemplate(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getReturnType(CXCursor C);

CLANGSHARP_LINKAGE CXSourceRange clangsharp_Cursor_getSourceRange(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getSpecialization(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_StmtClass clangsharp_Cursor_getStmtClass(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplateArgument(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXSourceLocation clangsharp_Cursor_getTemplateArgumentLocLocation(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplateArgumentLocSourceDeclExpression(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplateArgumentLocSourceExpression(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplateArgumentLocSourceIntegralExpression(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplateArgumentLocSourceNullPtrExpression(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplatedDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplateInstantiationPattern(CXCursor C);

CLANGSHARP_LINKAGE CX_TemplateSpecializationKind clangsharp_Cursor_getTemplateSpecializationKind(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getTemplateTypeParmDepth(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getTemplateTypeParmIndex(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getThisObjectType(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getThisType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTrailingRequiresClause(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTypedefNameForAnonDecl(CXCursor C);

CLANGSHARP_LINKAGE CX_UnaryOperatorKind clangsharp_Cursor_getUnaryOpcode(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getUnaryOpcodeSpelling(CX_UnaryOperatorKind Op);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUnderlyingDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUninstantiatedDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE CX_TypeClass clangsharp_Type_getTypeClass(CXType CT);

#endif
