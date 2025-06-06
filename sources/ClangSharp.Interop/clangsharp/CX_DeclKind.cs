// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_DeclKind
{
    CX_DeclKind_Invalid,
    CX_DeclKind_TranslationUnit,
    CX_DeclKind_TopLevelStmt,
    CX_DeclKind_RequiresExprBody,
    CX_DeclKind_OutlinedFunction,
    CX_DeclKind_LinkageSpec,
    CX_DeclKind_ExternCContext,
    CX_DeclKind_Export,
    CX_DeclKind_Captured,
    CX_DeclKind_Block,
    CX_DeclKind_StaticAssert,
    CX_DeclKind_PragmaDetectMismatch,
    CX_DeclKind_PragmaComment,
    CX_DeclKind_ObjCPropertyImpl,
    CX_DeclKind_OMPThreadPrivate,
    CX_DeclKind_OMPRequires,
    CX_DeclKind_OMPAllocate,
    CX_DeclKind_ObjCMethod,
    CX_DeclKind_ObjCProtocol,
    CX_DeclKind_ObjCInterface,
    CX_DeclKind_ObjCImplementation,
    CX_DeclKind_ObjCCategoryImpl,
    CX_DeclKind_FirstObjCImpl = CX_DeclKind_ObjCImplementation,
    CX_DeclKind_LastObjCImpl = CX_DeclKind_ObjCCategoryImpl,
    CX_DeclKind_ObjCCategory,
    CX_DeclKind_FirstObjCContainer = CX_DeclKind_ObjCProtocol,
    CX_DeclKind_LastObjCContainer = CX_DeclKind_ObjCCategory,
    CX_DeclKind_Namespace,
    CX_DeclKind_HLSLBuffer,
    CX_DeclKind_OMPDeclareReduction,
    CX_DeclKind_OMPDeclareMapper,
    CX_DeclKind_UnresolvedUsingValue,
    CX_DeclKind_UnnamedGlobalConstant,
    CX_DeclKind_TemplateParamObject,
    CX_DeclKind_MSGuid,
    CX_DeclKind_IndirectField,
    CX_DeclKind_EnumConstant,
    CX_DeclKind_Function,
    CX_DeclKind_CXXMethod,
    CX_DeclKind_CXXDestructor,
    CX_DeclKind_CXXConversion,
    CX_DeclKind_CXXConstructor,
    CX_DeclKind_FirstCXXMethod = CX_DeclKind_CXXMethod,
    CX_DeclKind_LastCXXMethod = CX_DeclKind_CXXConstructor,
    CX_DeclKind_CXXDeductionGuide,
    CX_DeclKind_FirstFunction = CX_DeclKind_Function,
    CX_DeclKind_LastFunction = CX_DeclKind_CXXDeductionGuide,
    CX_DeclKind_Var,
    CX_DeclKind_VarTemplateSpecialization,
    CX_DeclKind_VarTemplatePartialSpecialization,
    CX_DeclKind_FirstVarTemplateSpecialization = CX_DeclKind_VarTemplateSpecialization,
    CX_DeclKind_LastVarTemplateSpecialization = CX_DeclKind_VarTemplatePartialSpecialization,
    CX_DeclKind_ParmVar,
    CX_DeclKind_OMPCapturedExpr,
    CX_DeclKind_ImplicitParam,
    CX_DeclKind_Decomposition,
    CX_DeclKind_FirstVar = CX_DeclKind_Var,
    CX_DeclKind_LastVar = CX_DeclKind_Decomposition,
    CX_DeclKind_NonTypeTemplateParm,
    CX_DeclKind_MSProperty,
    CX_DeclKind_Field,
    CX_DeclKind_ObjCIvar,
    CX_DeclKind_ObjCAtDefsField,
    CX_DeclKind_FirstField = CX_DeclKind_Field,
    CX_DeclKind_LastField = CX_DeclKind_ObjCAtDefsField,
    CX_DeclKind_FirstDeclarator = CX_DeclKind_Function,
    CX_DeclKind_LastDeclarator = CX_DeclKind_ObjCAtDefsField,
    CX_DeclKind_Binding,
    CX_DeclKind_FirstValue = CX_DeclKind_OMPDeclareReduction,
    CX_DeclKind_LastValue = CX_DeclKind_Binding,
    CX_DeclKind_UsingShadow,
    CX_DeclKind_ConstructorUsingShadow,
    CX_DeclKind_FirstUsingShadow = CX_DeclKind_UsingShadow,
    CX_DeclKind_LastUsingShadow = CX_DeclKind_ConstructorUsingShadow,
    CX_DeclKind_UsingPack,
    CX_DeclKind_UsingDirective,
    CX_DeclKind_UnresolvedUsingIfExists,
    CX_DeclKind_Record,
    CX_DeclKind_CXXRecord,
    CX_DeclKind_ClassTemplateSpecialization,
    CX_DeclKind_ClassTemplatePartialSpecialization,
    CX_DeclKind_FirstClassTemplateSpecialization = CX_DeclKind_ClassTemplateSpecialization,
    CX_DeclKind_LastClassTemplateSpecialization = CX_DeclKind_ClassTemplatePartialSpecialization,
    CX_DeclKind_FirstCXXRecord = CX_DeclKind_CXXRecord,
    CX_DeclKind_LastCXXRecord = CX_DeclKind_ClassTemplatePartialSpecialization,
    CX_DeclKind_FirstRecord = CX_DeclKind_Record,
    CX_DeclKind_LastRecord = CX_DeclKind_ClassTemplatePartialSpecialization,
    CX_DeclKind_Enum,
    CX_DeclKind_FirstTag = CX_DeclKind_Record,
    CX_DeclKind_LastTag = CX_DeclKind_Enum,
    CX_DeclKind_UnresolvedUsingTypename,
    CX_DeclKind_Typedef,
    CX_DeclKind_TypeAlias,
    CX_DeclKind_ObjCTypeParam,
    CX_DeclKind_FirstTypedefName = CX_DeclKind_Typedef,
    CX_DeclKind_LastTypedefName = CX_DeclKind_ObjCTypeParam,
    CX_DeclKind_TemplateTypeParm,
    CX_DeclKind_FirstType = CX_DeclKind_Record,
    CX_DeclKind_LastType = CX_DeclKind_TemplateTypeParm,
    CX_DeclKind_TemplateTemplateParm,
    CX_DeclKind_VarTemplate,
    CX_DeclKind_TypeAliasTemplate,
    CX_DeclKind_FunctionTemplate,
    CX_DeclKind_ClassTemplate,
    CX_DeclKind_FirstRedeclarableTemplate = CX_DeclKind_VarTemplate,
    CX_DeclKind_LastRedeclarableTemplate = CX_DeclKind_ClassTemplate,
    CX_DeclKind_Concept,
    CX_DeclKind_BuiltinTemplate,
    CX_DeclKind_FirstTemplate = CX_DeclKind_TemplateTemplateParm,
    CX_DeclKind_LastTemplate = CX_DeclKind_BuiltinTemplate,
    CX_DeclKind_ObjCProperty,
    CX_DeclKind_ObjCCompatibleAlias,
    CX_DeclKind_NamespaceAlias,
    CX_DeclKind_Label,
    CX_DeclKind_UsingEnum,
    CX_DeclKind_Using,
    CX_DeclKind_FirstBaseUsing = CX_DeclKind_UsingEnum,
    CX_DeclKind_LastBaseUsing = CX_DeclKind_Using,
    CX_DeclKind_FirstNamed = CX_DeclKind_ObjCMethod,
    CX_DeclKind_LastNamed = CX_DeclKind_Using,
    CX_DeclKind_LifetimeExtendedTemporary,
    CX_DeclKind_Import,
    CX_DeclKind_ImplicitConceptSpecialization,
    CX_DeclKind_FriendTemplate,
    CX_DeclKind_Friend,
    CX_DeclKind_FileScopeAsm,
    CX_DeclKind_Empty,
    CX_DeclKind_AccessSpec,
    CX_DeclKind_FirstDecl = CX_DeclKind_TranslationUnit,
    CX_DeclKind_LastDecl = CX_DeclKind_AccessSpec,
}
