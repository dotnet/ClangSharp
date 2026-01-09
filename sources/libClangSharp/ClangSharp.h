// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

#ifndef LIBCLANGSHARP_CLANGSHARP_H
#define LIBCLANGSHARP_CLANGSHARP_H

#ifdef _MSC_VER
#pragma warning(push)
#pragma warning(disable : 4146 4244 4267 4291 4624 4996)
#endif

#include <clang/AST/Decl.h>
#include <clang/AST/DeclCXX.h>
#include <clang/AST/DeclObjC.h>
#include <clang/AST/Expr.h>
#include <clang/AST/ExprCXX.h>
#include <clang/AST/ExprObjC.h>
#include <clang/AST/Stmt.h>
#include <clang/AST/StmtCXX.h>
#include <clang/AST/StmtObjC.h>
#include <clang/AST/VTableBuilder.h>
#include <clang/Basic/Specifiers.h>
#include <clang-c/ExternC.h>
#include <clang-c/Index.h>

#ifdef _MSC_VER
#pragma warning(pop)
#endif

#include "ClangSharp_export.h"

enum CX_AtomicOperatorKind {
    CX_AO_Invalid,
#define BUILTIN(ID, TYPE, ATTRS)
#define ATOMIC_BUILTIN(ID, TYPE, ATTRS) CX_AO##ID,
#include <clang/Basic/Builtins.inc>
};

enum CX_AttrKind {
    CX_AttrKind_Invalid,
#define ATTR(X) CX_AttrKind_##X,
#define ATTR_RANGE(CLASS, FIRST_NAME, LAST_NAME) CX_AttrKind_First##CLASS = CX_AttrKind_##FIRST_NAME, CX_AttrKind_Last##CLASS = CX_AttrKind_##LAST_NAME,
#include <clang/Basic/AttrList.inc>
};


enum CX_CapturedRegionKind {
    CX_CR_Invalid,
    CX_CR_Default = clang::CR_Default + 1,
    CX_CR_ObjCAtFinally = clang::CR_ObjCAtFinally + 1,
    CX_CR_OpenMP = clang::CR_OpenMP + 1,
};

enum CX_CastKind {
    CX_CK_Invalid,
#define CAST_OPERATION(Name) CX_CK_##Name,
#include <clang/AST/OperationKinds.def>
};

enum CX_CharacterKind {
    CX_CLK_Invalid,
    CX_CLK_Ascii = static_cast<int>(clang::CharacterLiteralKind::Ascii) + 1,
    CX_CLK_Wide = static_cast<int>(clang::CharacterLiteralKind::Wide) + 1,
    CX_CLK_UTF8 = static_cast<int>(clang::CharacterLiteralKind::UTF8) + 1,
    CX_CLK_UTF16 = static_cast<int>(clang::CharacterLiteralKind::UTF16) + 1,
    CX_CLK_UTF32 = static_cast<int>(clang::CharacterLiteralKind::UTF32) + 1,
};

enum CX_ConstructionKind {
    _CX_CK_Invalid,
    CX_CK_Complete = static_cast<int>(clang::CXXConstructionKind::Complete) + 1,
    CX_CK_NonVirtualBase = static_cast<int>(clang::CXXConstructionKind::NonVirtualBase) + 1,
    CX_CK_VirtualBase = static_cast<int>(clang::CXXConstructionKind::VirtualBase) + 1,
    CX_CK_Delegating = static_cast<int>(clang::CXXConstructionKind::Delegating) + 1
};

enum CX_DeclKind {
    CX_DeclKind_Invalid,
#define DECL(DERIVED, BASE) CX_DeclKind_##DERIVED,
#define DECL_RANGE(BASE, START, END) CX_DeclKind_First##BASE = CX_DeclKind_##START, CX_DeclKind_Last##BASE = CX_DeclKind_##END,
#define LAST_DECL_RANGE(BASE, START, END) CX_DeclKind_First##BASE = CX_DeclKind_##START, CX_DeclKind_Last##BASE = CX_DeclKind_##END
#define ABSTRACT_DECL(DECL)
#include <clang/AST/DeclNodes.inc>
};

enum CX_ExprDependence {
    CX_ED_None = clang::ExprDependenceScope::None,
    CX_ED_UnexpandedPack = clang::ExprDependenceScope::UnexpandedPack,
    CX_ED_Instantiation = clang::ExprDependenceScope::Instantiation,
    CX_ED_Type = clang::ExprDependenceScope::Type,
    CX_ED_Value = clang::ExprDependenceScope::Value,
    CX_ED_Error = clang::ExprDependenceScope::Error,
    CX_ED_All = clang::ExprDependenceScope::All,

    CX_ED_TypeValue = clang::ExprDependenceScope::TypeValue,
    CX_ED_TypeInstantiation = clang::ExprDependenceScope::TypeInstantiation,
    CX_ED_ValueInstantiation = clang::ExprDependenceScope::ValueInstantiation,
    CX_ED_TypeValueInstantiation = clang::ExprDependenceScope::TypeValueInstantiation,
    CX_ED_ErrorDependent = clang::ExprDependenceScope::ErrorDependent,
};

enum CX_FloatingSemantics {
    CX_FLK_Invalid,
    CX_FLK_IEEEhalf = llvm::APFloatBase::S_IEEEhalf + 1,
    CX_FLK_BFloat = llvm::APFloatBase::S_BFloat + 1,
    CX_FLK_IEEEsingle = llvm::APFloatBase::S_IEEEsingle + 1,
    CX_FLK_IEEEdouble = llvm::APFloatBase::S_IEEEdouble + 1,
    CX_FLK_IEEEquad = llvm::APFloatBase::S_IEEEquad + 1,
    CX_FLK_PPCDoubleDouble = llvm::APFloatBase::S_PPCDoubleDouble + 1,
    CX_FLK_PPCDoubleDoubleLegacy = llvm::APFloatBase::S_PPCDoubleDoubleLegacy + 1,
    CX_FLK_Float8E5M2 = llvm::APFloatBase::S_Float8E5M2 + 1,
    CX_FLK_Float8E5M2FNUZ = llvm::APFloatBase::S_Float8E5M2FNUZ + 1,
    CX_FLK_Float8E4M3 = llvm::APFloatBase::S_Float8E4M3 + 1,
    CX_FLK_Float8E4M3FN = llvm::APFloatBase::S_Float8E4M3FN + 1,
    CX_FLK_Float8E4M3FNUZ = llvm::APFloatBase::S_Float8E4M3FNUZ + 1,
    CX_FLK_Float8E4M3B11FNUZ = llvm::APFloatBase::S_Float8E4M3B11FNUZ + 1,
    CX_FLK_Float8E3M4 = llvm::APFloatBase::S_Float8E3M4 + 1,
    CX_FLK_FloatTF32 = llvm::APFloatBase::S_FloatTF32 + 1,
    CX_FLK_Float8E8M0FNU = llvm::APFloatBase::S_Float8E8M0FNU + 1,
    CX_FLK_Float6E3M2FN = llvm::APFloatBase::S_Float6E3M2FN + 1,
    CX_FLK_Float6E2M3FN = llvm::APFloatBase::S_Float6E2M3FN + 1,
    CX_FLK_Float4E2M1FN = llvm::APFloatBase::S_Float4E2M1FN + 1,
    CX_FLK_x87DoubleExtended = llvm::APFloatBase::S_x87DoubleExtended + 1,
    CX_FLK_MaxSemantics = llvm::APFloatBase::S_MaxSemantics + 1,
};

enum CX_OverloadedOperatorKind {
    CX_OO_Invalid = clang::OO_None,
#define OVERLOADED_OPERATOR(Name,Spelling,Token,Unary,Binary,MemberOnly) \
    CX_OO_##Name = clang::OO_##Name,
#include "clang/Basic/OperatorKinds.def"
};

enum CX_StmtClass {
    CX_StmtClass_Invalid = clang::Stmt::NoStmtClass,
#define STMT(CLASS, PARENT) CX_StmtClass_##CLASS,
#define STMT_RANGE(BASE, FIRST, LAST) CX_StmtClass_First##BASE = CX_StmtClass_##FIRST, CX_StmtClass_Last##BASE = CX_StmtClass_##LAST,
#define LAST_STMT_RANGE(BASE, FIRST, LAST) CX_StmtClass_First##BASE = CX_StmtClass_##FIRST, CX_StmtClass_Last##BASE = CX_StmtClass_##LAST
#define ABSTRACT_STMT(STMT)
#include <clang/AST/StmtNodes.inc>
};

enum CX_StringKind {
    CX_SLK_Invalid,
    CX_SLK_Ordinary = static_cast<int>(clang::StringLiteralKind::Ordinary) + 1,
    CX_SLK_Wide = static_cast<int>(clang::StringLiteralKind::Wide) + 1,
    CX_SLK_UTF8 = static_cast<int>(clang::StringLiteralKind::UTF8) + 1,
    CX_SLK_UTF16 = static_cast<int>(clang::StringLiteralKind::UTF16) + 1,
    CX_SLK_UTF32 = static_cast<int>(clang::StringLiteralKind::UTF32) + 1,
    CX_SLK_Unevaluated = static_cast<int>(clang::StringLiteralKind::Unevaluated) + 1,
};

enum CX_TemplateArgumentDependence {
    CX_TAD_None = clang::TemplateArgumentDependenceScope::None,
    CX_TAD_UnexpandedPack = clang::TemplateArgumentDependenceScope::UnexpandedPack,
    CX_TAD_Instantiation = clang::TemplateArgumentDependenceScope::Instantiation,
    CX_TAD_Dependent = clang::TemplateArgumentDependenceScope::Dependent,
    CX_TAD_Error = clang::TemplateArgumentDependenceScope::Error,
    CX_TAD_DependentInstantiation = clang::TemplateArgumentDependenceScope::DependentInstantiation,
    CX_TAD_All = clang::TemplateArgumentDependenceScope::All
};

enum CX_TemplateNameKind {
    CX_TNK_Invalid,
    CX_TNK_Template = clang::TemplateName::Template + 1,
    CX_TNK_OverloadedTemplate = clang::TemplateName::OverloadedTemplate + 1,
    CX_TNK_AssumedTemplate = clang::TemplateName::AssumedTemplate + 1,
    CX_TNK_QualifiedTemplate = clang::TemplateName::QualifiedTemplate + 1,
    CX_TNK_DependentTemplate = clang::TemplateName::DependentTemplate + 1,
    CX_TNK_SubstTemplateTemplateParm = clang::TemplateName::SubstTemplateTemplateParm + 1,
    CX_TNK_SubstTemplateTemplateParmPack = clang::TemplateName::SubstTemplateTemplateParmPack + 1,
    CX_TNK_UsingTemplate = clang::TemplateName::UsingTemplate + 1,
    CX_TNK_DeducedTemplate = clang::TemplateName::DeducedTemplate + 1,
};

enum CX_TemplateSpecializationKind {
    CX_TSK_Invalid,
    CX_TSK_Undeclared = clang::TSK_Undeclared + 1,
    CX_TSK_ImplicitInstantiation = clang::TSK_ImplicitInstantiation + 1,
    CX_TSK_ExplicitSpecialization = clang::TSK_ExplicitSpecialization + 1,
    CX_TSK_ExplicitInstantiationDeclaration = clang::TSK_ExplicitInstantiationDeclaration + 1,
    CX_TSK_ExplicitInstantiationDefinition = clang::TSK_ExplicitInstantiationDefinition + 1,
};

enum CX_TypeClass {
    CX_TypeClass_Invalid,
#define TYPE(Class, Base) CX_TypeClass_##Class,
#define LAST_TYPE(Class) CX_TypeClass_TypeLast = CX_TypeClass_##Class,
#define ABSTRACT_TYPE(Class, Base)
#include <clang/AST/TypeNodes.inc>
    CX_TypeClass_TagFirst = CX_TypeClass_Record, CX_TypeClass_TagLast = CX_TypeClass_Enum
};

enum CX_UnaryExprOrTypeTrait {
    CX_UETT_Invalid,
 #define UNARY_EXPR_OR_TYPE_TRAIT(Spelling, Name, Key) CX_UETT_##Name,
 #define CXX11_UNARY_EXPR_OR_TYPE_TRAIT(Spelling, Name, Key) CX_UETT_##Name,
 #include "clang/Basic/TokenKinds.def"
    CX_UETT_Last = -1 // CX_UETT_Last == last CX_UETT_XX in the enum.
 #define UNARY_EXPR_OR_TYPE_TRAIT(Spelling, Name, Key) +1
 #define CXX11_UNARY_EXPR_OR_TYPE_TRAIT(Spelling, Name, Key) +1
 #include "clang/Basic/TokenKinds.def"
};

enum CX_VariableCaptureKind {
    CX_VCK_Invalid,
    CX_VCK_This = clang::CapturedStmt::VCK_This + 1,
    CX_VCK_ByRef = clang::CapturedStmt::VCK_ByRef + 1,
    CX_VCK_ByCopy = clang::CapturedStmt::VCK_ByCopy + 1,
    CX_VCK_VLAType = clang::CapturedStmt::VCK_VLAType + 1
};

enum CX_DestructorType {
    Deleting = clang::Dtor_Deleting,
    Complete = clang::Dtor_Complete,
    Base = clang::Dtor_Base,
    Comdat = clang::Dtor_Comdat
};

struct CX_TemplateArgument {
    CXTemplateArgumentKind kind;
    int xdata;
    const clang::TemplateArgument* value;
    CXTranslationUnit tu;
};

struct CX_TemplateArgumentLoc {
    const clang::TemplateArgumentLoc* value;
    CXTranslationUnit tu;
};

struct CX_TemplateName {
    CX_TemplateNameKind kind;
    const void* value;
    CXTranslationUnit tu;
};

LLVM_CLANG_C_EXTERN_C_BEGIN
CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getArgument(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getArgumentType(CXCursor C);

CLANGSHARP_LINKAGE int64_t clangsharp_Cursor_getArraySize(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAssociatedConstraint(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAsFunction(CXCursor C);

CLANGSHARP_LINKAGE CX_AtomicOperatorKind clangsharp_Cursor_getAtomicOpcode(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAttr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_AttrKind clangsharp_Cursor_getAttrKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBase(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXBinaryOperatorKind clangsharp_Cursor_getBinaryOpcode(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBindingDecl(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBindingExpr(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBitWidth(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBlockManglingContextDecl(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getBlockManglingNumber(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getBlockMissingReturnType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getBody(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getCallResultType(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCanAvoidCopyToHeap(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCanonical(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCaptureCopyExpr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCapturedVar(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_VariableCaptureKind clangsharp_Cursor_getCaptureKind(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureHasCopyExpr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureIsByRef(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureIsEscapingByRef(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureIsNested(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCaptureIsNonEscapingByRef(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCapturedDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCapturedRecordDecl(CXCursor C);

CLANGSHARP_LINKAGE CX_CapturedRegionKind clangsharp_Cursor_getCapturedRegionKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCapturedStmt(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCapturesCXXThis(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCapturesVariable(CXCursor C, CXCursor V);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCaptureVariable(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_CastKind clangsharp_Cursor_getCastKind(CXCursor C);

CLANGSHARP_LINKAGE CX_CharacterKind clangsharp_Cursor_getCharacterLiteralKind(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCharacterLiteralValue(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getChild(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getComputationLhsType(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getComputationResultType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getConstraintExpr(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getConstructedBaseClass(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getConstructedBaseClassShadowDecl(CXCursor C);

CLANGSHARP_LINKAGE CX_ConstructionKind clangsharp_Cursor_getConstructionKind(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getConstructsVirtualBase(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getContextParam(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getContextParamPosition(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCtor(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCXXRecord_IsPOD(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getBoolLiteralValue(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getDeclaredReturnType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDecl(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_DeclKind clangsharp_Cursor_getDeclKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDecomposedDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getDefaultArgType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDefinition(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDependentLambdaCallOperator(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDescribedCursorTemplate(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDescribedTemplate(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDestructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getDoesNotEscape(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getDoesUsualArrayDeleteWantSize(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getEnumDeclPromotionType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getEnumerator(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getExpansionType(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getExpr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_ExprDependence clangsharp_Cursor_getExprDependence(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getFieldIndex(CXCursor C);

CLANGSHARP_LINKAGE CX_FloatingSemantics clangsharp_Cursor_getFloatingLiteralSemantics(CXCursor C);

CLANGSHARP_LINKAGE double clangsharp_Cursor_getFloatingLiteralValueAsApproximateDouble(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getFoundDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getField(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getFriend(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getFriendDecl(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getFunctionScopeDepth(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getFunctionScopeIndex(CXCursor C);

CLANGSHARP_LINKAGE clang::MSGuidDeclParts clangsharp_Cursor_getGuidValue(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHadMultipleCandidates(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasBody(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUnparsedDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUninstantiatedDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasElseStorage(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasExplicitTemplateArgs(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasImplicitReturnZero(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasInheritedDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasInit(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasInitStorage(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasLeadingEmptyMacro(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasLocalStorage(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasPlaceholderTypeConstraint(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasTemplateKeyword(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUserDeclaredConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUserDeclaredCopyAssignment(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUserDeclaredCopyConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUserDeclaredDestructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUserDeclaredMoveAssignment(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUserDeclaredMoveConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasUserDeclaredMoveOperation(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasVarStorage(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getHoldingVar(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInClassInitializer(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInheritedConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getInheritedFromVBase(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInitExpr(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getInjectedSpecializationType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInstantiatedFromMember(CXCursor C);

CLANGSHARP_LINKAGE int64_t clangsharp_Cursor_getIntegerLiteralValue(CXCursor C);

CLANGSHARP_LINKAGE uint64_t clangsharp_Cursor_getUnsignedIntegerLiteralValue(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsAllEnumCasesCovered(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsAlwaysNull(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsAnonymousStructOrUnion(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsArgumentType(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsArrayForm(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsArrayFormAsWritten(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsArrow(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsCBuffer(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsClassExtension(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsCompleteDefinition(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsConditionTrue(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsConstexpr(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsConversionFromLambda(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsCopyOrMoveConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsCXXTry(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsDefined(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsDelegatingConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsDeleted(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsDeprecated(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsElidable(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsExpandedParameterPack(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsExplicitlyDefaulted(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsExternC(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsFileScope(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsGlobal(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsInjectedClassName(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsIfExists(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsImplicit(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsIncomplete(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsInheritingConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsListInitialization(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsLocalVarDecl(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsLocalVarDeclOrParm(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsMemberSpecialization(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsNegative(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsNonNegative(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsNoReturn(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsNothrow(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsOverloadedOperator(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsPackExpansion(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsParameterPack(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsPartiallySubstituted(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsPotentiallyEvaluated(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsPureVirtual(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsResultDependent(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsReversed(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsSigned(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsStatic(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsStaticDataMember(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsStdInitListInitialization(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsStrictlyPositive(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsTemplated(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsThisDeclarationADefinition(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsPropertyAccessor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsThrownVariableInScope(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsTransparent(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsTypeConcept(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsUnavailable(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsUnconditionallyVisible(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsUnnamedBitfield(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsUnsigned(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsUnsupportedFriend(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsUserProvided(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsVariadic(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaCallOperator(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaContextDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaStaticInvoker(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLhsExpr(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getMaxAlignment(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getMethod(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getMostRecentDecl(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getName(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNextDeclInContext(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNextSwitchCase(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNominatedBaseClass(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNominatedBaseClassShadowDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNonClosureContext(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumArguments(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumAssocs(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumAssociatedConstraints(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumAttrs(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumBases(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumBindings(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumCaptures(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumChildren(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumCtors(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumDecls(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumEnumerators(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumExpansionTypes(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumExprs(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumExprsOther(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumFields(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumFriends(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumMethods(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumProtocols(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumSpecializations(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumTemplateArguments(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumTemplateParameters(CXCursor C, unsigned listIndex);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumTemplateParameterLists(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumTypeParams(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumVBases(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getObjCRuntimeNameAttrMetadataName(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getOpaqueValue(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getOriginalType(CXCursor C);

CLANGSHARP_LINKAGE CX_OverloadedOperatorKind clangsharp_Cursor_getOverloadedOperatorKind(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getPackLength(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getParentFunctionOrMethod(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPlaceholderTypeConstraint(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPreviousDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPrimaryTemplate(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getProtocol(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getRedeclContext(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getReferenced(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getRequiresZeroInitialization(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getResultIndex(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getReturnType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getRhsExpr(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getSelector(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getShouldCopy(CXCursor C);

CLANGSHARP_LINKAGE CXSourceRange clangsharp_Cursor_getSourceRange(CXCursor C);

CLANGSHARP_LINKAGE CXSourceRange clangsharp_Cursor_getSourceRangeRaw(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getSpecialization(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_StmtClass clangsharp_Cursor_getStmtClass(CXCursor C);

CLANGSHARP_LINKAGE CX_StringKind clangsharp_Cursor_getStringLiteralKind(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getStringLiteralValue(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getSubDecl(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getSubExpr(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getSubExprAsWritten(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getSubStmt(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTargetUnionField(CXCursor C);

CLANGSHARP_LINKAGE CX_TemplateArgument clangsharp_Cursor_getTemplateArgument(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_TemplateArgumentLoc clangsharp_Cursor_getTemplateArgumentLoc(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplateParameter(CXCursor C, unsigned listIndex, unsigned parameterIndex);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplatedDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTemplateInstantiationPattern(CXCursor C);

CLANGSHARP_LINKAGE CX_TemplateSpecializationKind clangsharp_Cursor_getTemplateSpecializationKind(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getTemplateTypeParmDepth(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getTemplateTypeParmIndex(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getTemplateTypeParmPosition(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getThisObjectType(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getThisType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTrailingRequiresClause(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTypedefNameForAnonDecl(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getTypeOperand(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTypeParam(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_UnaryExprOrTypeTrait clangsharp_Cursor_getUnaryExprOrTypeTraitKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUnderlyingDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUninstantiatedDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUsedContext(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getTypeParamHasExplicitBound(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getTypeParamVariance(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getVBase(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE int64_t clangsharp_Cursor_getDtorVtblIdx(CXCursor C, CX_DestructorType dtor);

CLANGSHARP_LINKAGE int64_t clangsharp_Cursor_getVtblIdx(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_prettyPrintAttribute(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_getVersion();

CLANGSHARP_LINKAGE void clangsharp_TemplateArgument_dispose(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXCursor clangsharp_TemplateArgument_getAsDecl(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXCursor clangsharp_TemplateArgument_getAsExpr(CX_TemplateArgument T);

CLANGSHARP_LINKAGE int64_t clangsharp_TemplateArgument_getAsIntegral(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CX_TemplateName clangsharp_TemplateArgument_getAsTemplate(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CX_TemplateName clangsharp_TemplateArgument_getAsTemplateOrTemplatePattern(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXType clangsharp_TemplateArgument_getAsType(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CX_TemplateArgumentDependence clangsharp_TemplateArgument_getDependence(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXType clangsharp_TemplateArgument_getIntegralType(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXType clangsharp_TemplateArgument_getNonTypeTemplateArgumentType(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXType clangsharp_TemplateArgument_getNullPtrType(CX_TemplateArgument T);

CLANGSHARP_LINKAGE int clangsharp_TemplateArgument_getNumPackElements(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CX_TemplateArgument clangsharp_TemplateArgument_getPackElement(CX_TemplateArgument T, unsigned i);

CLANGSHARP_LINKAGE CX_TemplateArgument clangsharp_TemplateArgument_getPackExpansionPattern(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXType clangsharp_TemplateArgument_getParamTypeForDecl(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CX_TemplateArgument clangsharp_TemplateArgumentLoc_getArgument(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXSourceLocation clangsharp_TemplateArgumentLoc_getLocation(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXCursor clangsharp_TemplateArgumentLoc_getSourceDeclExpression(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXCursor clangsharp_TemplateArgumentLoc_getSourceExpression(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXCursor clangsharp_TemplateArgumentLoc_getSourceIntegralExpression(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXCursor clangsharp_TemplateArgumentLoc_getSourceNullPtrExpression(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXSourceRange clangsharp_TemplateArgumentLoc_getSourceRange(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXSourceRange clangsharp_TemplateArgumentLoc_getSourceRangeRaw(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXCursor clangsharp_TemplateName_getAsTemplateDecl(CX_TemplateName T);

CLANGSHARP_LINKAGE CXType clangsharp_Type_desugar(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getAddrSpaceExpr(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getAdjustedType(CXType CT);

CLANGSHARP_LINKAGE CX_AttrKind clangsharp_Type_getAttrKind(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getBaseType(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getColumnExpr(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getDecayedType(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getDeclaration(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getDeducedType(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getDepth(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getElementType(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getEquivalentType(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getIndex(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getInjectedSpecializationType(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getInjectedTST(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCInstanceType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsSigned(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsSugared(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsTypeAlias(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsUnsigned(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getModifiedType(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getNumBits(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getNumBitsExpr(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getNumColumns(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getNumElementsFlattened(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getNumRows(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getOriginalType(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getSubstTemplateTypeParamAssociatedDecl(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getOwnedTagDecl(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getPointeeType(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getRowExpr(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getSizeExpr(CXType CT);

CLANGSHARP_LINKAGE CX_TemplateArgument clangsharp_Type_getTemplateArgument(CXType C, unsigned i);

CLANGSHARP_LINKAGE CX_TemplateName clangsharp_Type_getTemplateName(CXType C);

CLANGSHARP_LINKAGE CX_TypeClass clangsharp_Type_getTypeClass(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getUnderlyingExpr(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getUnderlyingType(CXType CT);
LLVM_CLANG_C_EXTERN_C_END

#endif
