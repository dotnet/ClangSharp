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
#include <clang/Lex/MacroInfo.h>
#include <clang/Lex/Preprocessor.h>
#include <clang/Lex/PreprocessingRecord.h>
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

enum CX_ConstexprSpecKind {
    CX_CSK_Invalid,
    CX_CSK_Unspecified = static_cast<int>(clang::ConstexprSpecKind::Unspecified) + 1,
    CX_CSK_Constexpr = static_cast<int>(clang::ConstexprSpecKind::Constexpr) + 1,
    CX_CSK_Consteval = static_cast<int>(clang::ConstexprSpecKind::Consteval) + 1,
    CX_CSK_Constinit = static_cast<int>(clang::ConstexprSpecKind::Constinit) + 1,
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

enum CX_ExprObjectKind {
    CX_OK_Invalid,
    CX_OK_Ordinary = clang::OK_Ordinary + 1,
    CX_OK_BitField = clang::OK_BitField + 1,
    CX_OK_VectorComponent = clang::OK_VectorComponent + 1,
    CX_OK_ObjCProperty = clang::OK_ObjCProperty + 1,
    CX_OK_ObjCSubscript = clang::OK_ObjCSubscript + 1,
    CX_OK_MatrixComponent = clang::OK_MatrixComponent + 1,
};

enum CX_ExprValueKind {
    CX_VK_Invalid,
    CX_VK_PRValue = clang::VK_PRValue + 1,
    CX_VK_LValue = clang::VK_LValue + 1,
    CX_VK_XValue = clang::VK_XValue + 1,
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

enum CX_IfStatementKind {
    CX_ISK_Invalid,
    CX_ISK_Ordinary = static_cast<int>(clang::IfStatementKind::Ordinary) + 1,
    CX_ISK_Constexpr = static_cast<int>(clang::IfStatementKind::Constexpr) + 1,
    CX_ISK_ConstevalNonNegated = static_cast<int>(clang::IfStatementKind::ConstevalNonNegated) + 1,
    CX_ISK_ConstevalNegated = static_cast<int>(clang::IfStatementKind::ConstevalNegated) + 1,
};

enum CX_TypeDependence {
    CX_TD_None = clang::TypeDependenceScope::None,
    CX_TD_UnexpandedPack = clang::TypeDependenceScope::UnexpandedPack,
    CX_TD_Instantiation = clang::TypeDependenceScope::Instantiation,
    CX_TD_Dependent = clang::TypeDependenceScope::Dependent,
    CX_TD_VariablyModified = clang::TypeDependenceScope::VariablyModified,
    CX_TD_Error = clang::TypeDependenceScope::Error,
    CX_TD_All = clang::TypeDependenceScope::All,

    CX_TD_DependentInstantiation = clang::TypeDependenceScope::DependentInstantiation,
};

enum CX_ExceptionSpecificationType {
    CX_EST_Invalid,
    CX_EST_None = clang::EST_None + 1,
    CX_EST_DynamicNone = clang::EST_DynamicNone + 1,
    CX_EST_Dynamic = clang::EST_Dynamic + 1,
    CX_EST_MSAny = clang::EST_MSAny + 1,
    CX_EST_NoThrow = clang::EST_NoThrow + 1,
    CX_EST_BasicNoexcept = clang::EST_BasicNoexcept + 1,
    CX_EST_DependentNoexcept = clang::EST_DependentNoexcept + 1,
    CX_EST_NoexceptFalse = clang::EST_NoexceptFalse + 1,
    CX_EST_NoexceptTrue = clang::EST_NoexceptTrue + 1,
    CX_EST_Unevaluated = clang::EST_Unevaluated + 1,
    CX_EST_Uninstantiated = clang::EST_Uninstantiated + 1,
    CX_EST_Unparsed = clang::EST_Unparsed + 1,
};

enum CX_VectorKind {
    CX_VECK_Invalid,
    CX_VECK_Generic = static_cast<int>(clang::VectorKind::Generic) + 1,
    CX_VECK_AltiVecVector = static_cast<int>(clang::VectorKind::AltiVecVector) + 1,
    CX_VECK_AltiVecPixel = static_cast<int>(clang::VectorKind::AltiVecPixel) + 1,
    CX_VECK_AltiVecBool = static_cast<int>(clang::VectorKind::AltiVecBool) + 1,
    CX_VECK_Neon = static_cast<int>(clang::VectorKind::Neon) + 1,
    CX_VECK_NeonPoly = static_cast<int>(clang::VectorKind::NeonPoly) + 1,
    CX_VECK_SveFixedLengthData = static_cast<int>(clang::VectorKind::SveFixedLengthData) + 1,
    CX_VECK_SveFixedLengthPredicate = static_cast<int>(clang::VectorKind::SveFixedLengthPredicate) + 1,
    CX_VECK_RVVFixedLengthData = static_cast<int>(clang::VectorKind::RVVFixedLengthData) + 1,
    CX_VECK_RVVFixedLengthMask = static_cast<int>(clang::VectorKind::RVVFixedLengthMask) + 1,
    CX_VECK_RVVFixedLengthMask_1 = static_cast<int>(clang::VectorKind::RVVFixedLengthMask_1) + 1,
    CX_VECK_RVVFixedLengthMask_2 = static_cast<int>(clang::VectorKind::RVVFixedLengthMask_2) + 1,
    CX_VECK_RVVFixedLengthMask_4 = static_cast<int>(clang::VectorKind::RVVFixedLengthMask_4) + 1,
};

enum CX_AutoTypeKeyword {
    CX_ATK_Invalid,
    CX_ATK_Auto = static_cast<int>(clang::AutoTypeKeyword::Auto) + 1,
    CX_ATK_DecltypeAuto = static_cast<int>(clang::AutoTypeKeyword::DecltypeAuto) + 1,
    CX_ATK_GNUAutoType = static_cast<int>(clang::AutoTypeKeyword::GNUAutoType) + 1,
};

enum CX_ElaboratedTypeKeyword {
    CX_ETK_Invalid,
    CX_ETK_Struct = static_cast<int>(clang::ElaboratedTypeKeyword::Struct) + 1,
    CX_ETK_Interface = static_cast<int>(clang::ElaboratedTypeKeyword::Interface) + 1,
    CX_ETK_Union = static_cast<int>(clang::ElaboratedTypeKeyword::Union) + 1,
    CX_ETK_Class = static_cast<int>(clang::ElaboratedTypeKeyword::Class) + 1,
    CX_ETK_Enum = static_cast<int>(clang::ElaboratedTypeKeyword::Enum) + 1,
    CX_ETK_Typename = static_cast<int>(clang::ElaboratedTypeKeyword::Typename) + 1,
    CX_ETK_None = static_cast<int>(clang::ElaboratedTypeKeyword::None) + 1,
};

enum CX_ObjCMessageReceiverKind {
    CX_OMRK_Invalid,
    CX_OMRK_Class = static_cast<int>(clang::ObjCMessageExpr::Class) + 1,
    CX_OMRK_Instance = static_cast<int>(clang::ObjCMessageExpr::Instance) + 1,
    CX_OMRK_SuperClass = static_cast<int>(clang::ObjCMessageExpr::SuperClass) + 1,
    CX_OMRK_SuperInstance = static_cast<int>(clang::ObjCMessageExpr::SuperInstance) + 1,
};

enum CX_ObjCPropertyRefReceiverKind {
    CX_OPRK_Invalid,
    CX_OPRK_Object,
    CX_OPRK_Super,
    CX_OPRK_Class,
};

enum CX_InitializationStyle {
    CX_IS_Invalid,
    CX_IS_CInit = clang::VarDecl::CInit + 1,
    CX_IS_CallInit = clang::VarDecl::CallInit + 1,
    CX_IS_ListInit = clang::VarDecl::ListInit + 1,
    CX_IS_ParenListInit = clang::VarDecl::ParenListInit + 1,
};

enum CX_InclusionDirectiveKind {
    CX_IDK_Invalid,
    CX_IDK_Include = static_cast<int>(clang::InclusionDirective::Include) + 1,
    CX_IDK_Import = static_cast<int>(clang::InclusionDirective::Import) + 1,
    CX_IDK_IncludeNext = static_cast<int>(clang::InclusionDirective::IncludeNext) + 1,
    CX_IDK_IncludeMacros = static_cast<int>(clang::InclusionDirective::IncludeMacros) + 1,
};

enum CX_LambdaCaptureDefault {
    CX_LCD_Invalid,
    CX_LCD_None = clang::LCD_None + 1,
    CX_LCD_ByCopy = clang::LCD_ByCopy + 1,
    CX_LCD_ByRef = clang::LCD_ByRef + 1,
};

enum CX_LiteralOperatorKind {
    CX_LOK_Invalid,
    CX_LOK_Raw = clang::UserDefinedLiteral::LOK_Raw + 1,
    CX_LOK_Template = clang::UserDefinedLiteral::LOK_Template + 1,
    CX_LOK_Integer = clang::UserDefinedLiteral::LOK_Integer + 1,
    CX_LOK_Floating = clang::UserDefinedLiteral::LOK_Floating + 1,
    CX_LOK_String = clang::UserDefinedLiteral::LOK_String + 1,
    CX_LOK_Character = clang::UserDefinedLiteral::LOK_Character + 1,
};

enum CX_OverloadedOperatorKind {
    CX_OO_Invalid = clang::OO_None,
#define OVERLOADED_OPERATOR(Name,Spelling,Token,Unary,Binary,MemberOnly) \
    CX_OO_##Name = clang::OO_##Name,
#include "clang/Basic/OperatorKinds.def"
};

enum CX_PredefinedIdentKind {
    CX_PIK_Invalid,
    CX_PIK_Func = static_cast<int>(clang::PredefinedIdentKind::Func) + 1,
    CX_PIK_Function = static_cast<int>(clang::PredefinedIdentKind::Function) + 1,
    CX_PIK_LFunction = static_cast<int>(clang::PredefinedIdentKind::LFunction) + 1,
    CX_PIK_FuncDName = static_cast<int>(clang::PredefinedIdentKind::FuncDName) + 1,
    CX_PIK_FuncSig = static_cast<int>(clang::PredefinedIdentKind::FuncSig) + 1,
    CX_PIK_LFuncSig = static_cast<int>(clang::PredefinedIdentKind::LFuncSig) + 1,
    CX_PIK_PrettyFunction = static_cast<int>(clang::PredefinedIdentKind::PrettyFunction) + 1,
    CX_PIK_PrettyFunctionNoVirtual = static_cast<int>(clang::PredefinedIdentKind::PrettyFunctionNoVirtual) + 1,
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

enum CX_TypeTrait {
    CX_TT_Invalid,
#define TYPE_TRAIT_1(Spelling, Name, Key) CX_UTT_##Name,
#include "clang/Basic/TokenKinds.def"
    CX_UTT_Last = 0 // CX_UTT_Last == last CX_UTT_XX in the enum.
#define TYPE_TRAIT_1(Spelling, Name, Key) +1
#include "clang/Basic/TokenKinds.def"
    ,
#define TYPE_TRAIT_2(Spelling, Name, Key) CX_BTT_##Name,
#include "clang/Basic/TokenKinds.def"
    CX_BTT_Last = CX_UTT_Last // CX_BTT_Last == last CX_BTT_XX in the enum.
#define TYPE_TRAIT_2(Spelling, Name, Key) +1
#include "clang/Basic/TokenKinds.def"
    ,
#define TYPE_TRAIT_N(Spelling, Name, Key) CX_TT_##Name,
#include "clang/Basic/TokenKinds.def"
    CX_TT_Last = CX_BTT_Last // CX_TT_Last == last CX_TT_XX in the enum.
#define TYPE_TRAIT_N(Spelling, Name, Key) +1
#include "clang/Basic/TokenKinds.def"
};

enum CX_UnaryExprOrTypeTrait {
    CX_UETT_Invalid,
 #define UNARY_EXPR_OR_TYPE_TRAIT(Spelling, Name, Key) CX_UETT_##Name,
 #define CXX11_UNARY_EXPR_OR_TYPE_TRAIT(Spelling, Name, Key) CX_UETT_##Name,
 #include "clang/Basic/TokenKinds.def"
    CX_UETT_Last = 0 // CX_UETT_Last == last CX_UETT_XX in the enum.
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

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAllocate(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getAsmString(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAsmStringExpr(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getArgumentType(CXCursor C);

CLANGSHARP_LINKAGE int64_t clangsharp_Cursor_getArraySize(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAssociatedConstraint(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAsFunction(CXCursor C);

CLANGSHARP_LINKAGE CX_AtomicOperatorKind clangsharp_Cursor_getAtomicOpcode(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getAttr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_AttrKind clangsharp_Cursor_getAttrKind(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getAttrSpelling(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getAvailabilityAttributeDeprecated(CXCursor, llvm::VersionTuple* version);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getAvailabilityAttributeIntroduced(CXCursor, llvm::VersionTuple* version);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getAvailabilityAttributeMessage(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getAvailabilityAttributeObsoleted(CXCursor, llvm::VersionTuple* version);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getAvailabilityAttributePlatformIdentifierName(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getAvailabilityAttributeUnavailable(CXCursor);

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

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getClobber(CXCursor C, unsigned i);

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

CLANGSHARP_LINKAGE CX_ConstexprSpecKind clangsharp_Cursor_getConstexprKind(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getConstructsVirtualBase(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getContextParam(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getContextParamPosition(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCtor(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getCXXRecord_IsPOD(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getBoolLiteralValue(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getCookedLiteral(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getDeclaredReturnType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getDeallocate(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getExceptionHandler(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getFallthroughHandler(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getFinalSuspendStmt(CXCursor C);

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

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasAPValueResult(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasBody(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasBraces(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasDependentPromiseType(CXCursor C);

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

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasDeletedDestructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasInClassInitializer(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasMutableFields(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasNonTrivialDefaultConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasNonTrivialDestructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasPrivateFields(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasProtectedFields(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasTrivialCopyConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasTrivialDefaultConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasTrivialDestructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasVarStorage(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getHoldingVar(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInClassInitializer(CXCursor C);

CLANGSHARP_LINKAGE CX_PredefinedIdentKind clangsharp_Cursor_getIdentKind(CXCursor C);

CLANGSHARP_LINKAGE CX_IfStatementKind clangsharp_Cursor_getIfStatementKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getIgnoreParenNoopCasts(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInheritedConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getInheritedFromVBase(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInitExpr(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInitSuspendStmt(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getInputConstraint(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getInputExpr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getInputName(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_InitializationStyle clangsharp_Cursor_getInitStyle(CXCursor C);

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

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsFixed(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsGlobal(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsInjectedClassName(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsIfExists(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsImplicit(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsAsmGoto(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsSimple(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsVolatile(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsIncomplete(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsInherited(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsInheritingConstructor(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsAggregate(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsCapturelessLambda(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsCXX11StandardLayout(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsDynamicClass(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsEffectivelyFinal(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsEmpty(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsGenericLambda(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsLambda(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsLiteral(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsPolymorphic(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsStandardLayout(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsTrivial(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsTriviallyCopyable(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsLateParsed(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsListInitialization(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsLocalVarDecl(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsLocalVarDeclOrParm(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsMemberSpecialization(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsNegative(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsNested(CXCursor C);

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

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsZeroLengthBitField(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaCallOperator(CXCursor C);

CLANGSHARP_LINKAGE CX_LambdaCaptureDefault clangsharp_Cursor_getLambdaCaptureDefault(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaContextDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLambdaStaticInvoker(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLabelExpr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getLabelName(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getLhsExpr(CXCursor C);

CLANGSHARP_LINKAGE CX_LiteralOperatorKind clangsharp_Cursor_getLiteralOperatorKind(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getMaxAlignment(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getMethod(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getMethodFamily(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getMostRecentDecl(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getName(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNextDeclInContext(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNextSwitchCase(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNominatedBaseClass(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNominatedBaseClassShadowDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getNonClosureContext(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumArguments(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getNumClobbers(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getNumInputs(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getNumLabels(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getNumOutputs(CXCursor C);

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

CLANGSHARP_LINKAGE CX_ExprObjectKind clangsharp_Cursor_getObjectKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getOpaqueValue(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getOperand(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getOutputConstraint(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getOutputExpr(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getOutputName(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getOriginalType(CXCursor C);

CLANGSHARP_LINKAGE CX_OverloadedOperatorKind clangsharp_Cursor_getOverloadedOperatorKind(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getPackLength(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getParentFunctionOrMethod(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPromiseCall(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPromiseDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPlaceholderTypeConstraint(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPreviousDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getPrimaryTemplate(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getPropertyAttributes(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getProtocol(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getQualifiedName(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getRedeclContext(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getReferenced(CXCursor C);

CLANGSHARP_LINKAGE CXRefQualifierKind clangsharp_Cursor_getRefQualifier(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getRequiresZeroInitialization(CXCursor C);

CLANGSHARP_LINKAGE int64_t clangsharp_Cursor_getResultAsAPSInt(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getResultIndex(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getResultDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getReturnStmt(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getReturnStmtOnAllocFailure(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getReturnValue(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getReturnValueInit(CXCursor C);

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

CLANGSHARP_LINKAGE CX_InclusionDirectiveKind clangsharp_Cursor_getInclusionDirectiveKind(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getInclusionDirectiveImportedModule(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getInclusionDirectiveWasInQuotes(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsMacroC99Varargs(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsMacroGNUVarargs(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsMacroVariadic(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getMacroExpansionDefinition(CXCursor C);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getMacroParamName(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getMacroTokenKind(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXString clangsharp_Cursor_getMacroTokenSpelling(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumMacroParams(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumMacroTokens(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasCatchAll(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getHasObjCAvailabilityVersion(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsDelegateInitCall(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsMessagingGetter(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getIsMessagingSetter(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumCatchStmts(CXCursor C);

CLANGSHARP_LINKAGE int clangsharp_Cursor_getNumObjCLiteralElements(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getObjCAvailabilityVersion(CXCursor C, llvm::VersionTuple* version);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getObjCDictionaryKey(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getObjCDictionaryValue(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_ObjCMessageReceiverKind clangsharp_Cursor_getObjCMessageReceiverKind(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getObjCMessageReceiverType(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getObjCPropertyRefClassReceiver(CXCursor C);

CLANGSHARP_LINKAGE CX_ObjCPropertyRefReceiverKind clangsharp_Cursor_getObjCPropertyRefReceiverKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getSetAtIndexMethodDecl(CXCursor C);

CLANGSHARP_LINKAGE CXType clangsharp_Cursor_getTypeOperand(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getTypeParam(CXCursor C, unsigned i);

CLANGSHARP_LINKAGE CX_UnaryExprOrTypeTrait clangsharp_Cursor_getUnaryExprOrTypeTraitKind(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUnderlyingDecl(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUninstantiatedDefaultArg(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUsedContext(CXCursor C);

CLANGSHARP_LINKAGE CXCursor clangsharp_Cursor_getUsingEnumDeclEnumDecl(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getTypeParamHasExplicitBound(CXCursor C);

CLANGSHARP_LINKAGE unsigned clangsharp_Cursor_getTypeParamVariance(CXCursor C);

CLANGSHARP_LINKAGE CX_TypeTrait clangsharp_Cursor_getTypeTrait(CXCursor C);

CLANGSHARP_LINKAGE CX_ExprValueKind clangsharp_Cursor_getValueKind(CXCursor C);

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

CLANGSHARP_LINKAGE unsigned clangsharp_TemplateArgument_getIsDefaulted(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXType clangsharp_TemplateArgument_getNonTypeTemplateArgumentType(CX_TemplateArgument T);

CLANGSHARP_LINKAGE CXType clangsharp_TemplateArgument_getNullPtrType(CX_TemplateArgument T);

CLANGSHARP_LINKAGE int clangsharp_TemplateArgument_getNumPackElements(CX_TemplateArgument T);

CLANGSHARP_LINKAGE int clangsharp_TemplateArgument_getNumTemplateExpansions(CX_TemplateArgument T);

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

CLANGSHARP_LINKAGE CXSourceLocation clangsharp_TemplateArgumentLoc_getTemplateEllipsisLoc(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXSourceLocation clangsharp_TemplateArgumentLoc_getTemplateNameLoc(CX_TemplateArgumentLoc T);

CLANGSHARP_LINKAGE CXCursor clangsharp_TemplateName_getAsTemplateDecl(CX_TemplateName T);

CLANGSHARP_LINKAGE unsigned clangsharp_TemplateName_getContainsUnexpandedParameterPack(CX_TemplateName T);

CLANGSHARP_LINKAGE unsigned clangsharp_TemplateName_getIsDependent(CX_TemplateName T);

CLANGSHARP_LINKAGE unsigned clangsharp_TemplateName_getIsInstantiationDependent(CX_TemplateName T);

CLANGSHARP_LINKAGE CX_TemplateName clangsharp_TemplateName_getUnderlying(CX_TemplateName T);

CLANGSHARP_LINKAGE CXType clangsharp_Type_desugar(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getAddrSpaceExpr(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getAdjustedType(CXType CT);

CLANGSHARP_LINKAGE CX_AttrKind clangsharp_Type_getAttrKind(CXType CT);

CLANGSHARP_LINKAGE CX_AutoTypeKeyword clangsharp_Type_getAutoTypeKeyword(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getBaseType(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getColumnExpr(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getContainsUnexpandedParameterPack(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getDecayedType(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getDeclaration(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getDeducedType(CXType CT);

CLANGSHARP_LINKAGE CX_TypeDependence clangsharp_Type_getDependence(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getDepth(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getElementType(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getEquivalentType(CXType CT);

CLANGSHARP_LINKAGE CX_ExceptionSpecificationType clangsharp_Type_getExceptionSpecType(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getExceptionType(CXType C, unsigned i);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getHasFloatingRepresentation(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getHasIntegerRepresentation(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getHasPointerRepresentation(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getHasTrailingReturn(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getIndex(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getInjectedSpecializationType(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getInjectedTST(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsAggregateType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsArithmeticType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsConst(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsConstrained(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsDecltypeAuto(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsDependentType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsFloatingType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsGNUAutoType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsInstantiationDependentType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCInstanceType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjectType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsParameterPack(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsRealFloatingType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsScalarType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsSigned(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsSugared(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsTypeAlias(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsUnsigned(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsVariablyModifiedType(CXType CT);

CLANGSHARP_LINKAGE CX_ElaboratedTypeKeyword clangsharp_Type_getKeyword(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getModifiedType(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getNoexceptExpr(CXType CT);

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

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCClassType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCIdType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCKindOfType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCKindOfTypeAsWritten(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCObjectSpecialized(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCObjectSpecializedAsWritten(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCQualifiedClassType(CXType CT);

CLANGSHARP_LINKAGE unsigned clangsharp_Type_getIsObjCQualifiedIdType(CXType CT);

CLANGSHARP_LINKAGE int clangsharp_Type_getNumObjCTypeParamProtocols(CXType CT);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getObjCSuperClassType(CXType CT);

CLANGSHARP_LINKAGE CXCursor clangsharp_Type_getObjCTypeParamProtocol(CXType CT, unsigned i);

CLANGSHARP_LINKAGE CXType clangsharp_Type_getUnderlyingType(CXType CT);

CLANGSHARP_LINKAGE CX_VectorKind clangsharp_Type_getVectorKind(CXType CT);
LLVM_CLANG_C_EXTERN_C_END

#endif
