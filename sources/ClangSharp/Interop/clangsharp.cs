// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/master/sources/libClangSharp

using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public static partial class clangsharp
    {
        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getArgument", ExactSpelling = true)]
        public static extern CXCursor Cursor_getArgument(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getArgumentType", ExactSpelling = true)]
        public static extern CXType Cursor_getArgumentType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getArraySize", ExactSpelling = true)]
        [return: NativeTypeName("int64_t")]
        public static extern long Cursor_getArraySize(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAssociatedConstraint", ExactSpelling = true)]
        public static extern CXCursor Cursor_getAssociatedConstraint(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAsFunction", ExactSpelling = true)]
        public static extern CXCursor Cursor_getAsFunction(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAttrKind", ExactSpelling = true)]
        public static extern CX_AttrKind Cursor_getAttrKind(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBaseExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBaseExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinaryOpcode", ExactSpelling = true)]
        public static extern CX_BinaryOperatorKind Cursor_getBinaryOpcode(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinaryOpcodeSpelling", ExactSpelling = true)]
        public static extern CXString Cursor_getBinaryOpcodeSpelling(CX_BinaryOperatorKind Op);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinding", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBinding(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBitWidth", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBitWidth(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBlockManglingContextDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBlockManglingContextDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBlockManglingNumber", ExactSpelling = true)]
        public static extern int Cursor_getBlockManglingNumber(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBlockMissingReturnType", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getBlockMissingReturnType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBody", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBody(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCalleeExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getCalleeExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCallResultType", ExactSpelling = true)]
        public static extern CXType Cursor_getCallResultType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCanAvoidCopyToHeap", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCanAvoidCopyToHeap(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureCopyExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getCaptureCopyExpr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureHasCopyExpr", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCaptureHasCopyExpr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureIsByRef", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCaptureIsByRef(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureIsEscapingByRef", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCaptureIsEscapingByRef(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureIsNested", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCaptureIsNested(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureIsNonEscapingByRef", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCaptureIsNonEscapingByRef(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturesCXXThis", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCapturesCXXThis(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturesVariable", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCapturesVariable(CXCursor C, CXCursor V);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureVariable", ExactSpelling = true)]
        public static extern CXCursor Cursor_getCaptureVariable(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCastKind", ExactSpelling = true)]
        public static extern CX_CastKind Cursor_getCastKind(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCharacterLiteralKind", ExactSpelling = true)]
        public static extern CX_CharacterKind Cursor_getCharacterLiteralKind(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCharacterLiteralValue", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCharacterLiteralValue(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCommonExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getCommonExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getComputationLhsType", ExactSpelling = true)]
        public static extern CXType Cursor_getComputationLhsType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getComputationResultType", ExactSpelling = true)]
        public static extern CXType Cursor_getComputationResultType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCondExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getCondExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConditionVariableDeclStmt", ExactSpelling = true)]
        public static extern CXCursor Cursor_getConditionVariableDeclStmt(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstraintExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getConstraintExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructedBaseClass", ExactSpelling = true)]
        public static extern CXCursor Cursor_getConstructedBaseClass(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructedBaseClassShadowDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getConstructedBaseClassShadowDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructsVirtualBase", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getConstructsVirtualBase(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getContextParam", ExactSpelling = true)]
        public static extern CXCursor Cursor_getContextParam(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getContextParamPosition", ExactSpelling = true)]
        public static extern int Cursor_getContextParamPosition(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConversionFunction", ExactSpelling = true)]
        public static extern CXCursor Cursor_getConversionFunction(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCXXBoolLiteralExprValue", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getCXXBoolLiteralExprValue(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDeclaredReturnType", ExactSpelling = true)]
        public static extern CXType Cursor_getDeclaredReturnType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDeclKind", ExactSpelling = true)]
        public static extern CX_DeclKind Cursor_getDeclKind(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDecomposedDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDecomposedDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDefaultArg", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDefaultArg(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDefaultArgType", ExactSpelling = true)]
        public static extern CXType Cursor_getDefaultArgType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDependentLambdaCallOperator", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDependentLambdaCallOperator(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDescribedClassTemplate", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDescribedClassTemplate(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDescribedTemplate", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDescribedTemplate(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDestructor", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDestructor(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDirectCallee", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDirectCallee(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDoesNotEscape", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getDoesNotEscape(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getEnumDeclPromotionType", ExactSpelling = true)]
        public static extern CXType Cursor_getEnumDeclPromotionType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getExpr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFalseExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getFalseExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFieldIndex", ExactSpelling = true)]
        public static extern int Cursor_getFieldIndex(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFloatingLiteralSemantics", ExactSpelling = true)]
        public static extern CX_FloatingSemantics Cursor_getFloatingLiteralSemantics(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFloatingLiteralValueAsApproximateDouble", ExactSpelling = true)]
        public static extern double Cursor_getFloatingLiteralValueAsApproximateDouble(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFriendDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getFriendDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFunctionScopeDepth", ExactSpelling = true)]
        public static extern int Cursor_getFunctionScopeDepth(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFunctionScopeIndex", ExactSpelling = true)]
        public static extern int Cursor_getFunctionScopeIndex(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFunctionType", ExactSpelling = true)]
        public static extern CXType Cursor_getFunctionType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasBody", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasBody(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasDefaultArg", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasDefaultArg(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasExplicitTemplateArgs", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasExplicitTemplateArgs(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasExternalStorage", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasExternalStorage(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasGlobalStorage", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasGlobalStorage(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasImplicitReturnZero", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasImplicitReturnZero(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasInheritedDefaultArg", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasInheritedDefaultArg(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasInit", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasInit(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasLocalStorage", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasLocalStorage(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasPlaceholderTypeConstraint", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getHasPlaceholderTypeConstraint(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHoldingVar", ExactSpelling = true)]
        public static extern CXCursor Cursor_getHoldingVar(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIdxExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getIdxExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIncExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getIncExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInClassInitializer", ExactSpelling = true)]
        public static extern CXCursor Cursor_getInClassInitializer(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInitExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getInitExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInstantiatedFromMember", ExactSpelling = true)]
        public static extern CXCursor Cursor_getInstantiatedFromMember(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIntegerLiteralValue", ExactSpelling = true)]
        [return: NativeTypeName("int64_t")]
        public static extern long Cursor_getIntegerLiteralValue(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsAnonymousStructOrUnion", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsAnonymousStructOrUnion(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsArgumentType", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsArgumentType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsConversionFromLambda", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsConversionFromLambda(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDefined", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsDefined(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDeprecated", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsDeprecated(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsExpandedParameterPack", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsExpandedParameterPack(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsExternC", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsExternC(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsGlobal", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsGlobal(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsImplicitAccess", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsImplicitAccess(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsLocalVarDecl", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsLocalVarDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsLocalVarDeclOrParm", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsLocalVarDeclOrParm(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsMemberSpecialization", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsMemberSpecialization(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNegative", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsNegative(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNonNegative", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsNonNegative(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNoReturn", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsNoReturn(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNothrow", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsNothrow(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsOverloadedOperator", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsOverloadedOperator(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPackExpansion", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsPackExpansion(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsParameterPack", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsParameterPack(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPure", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsPure(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsSigned", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsSigned(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStatic", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsStatic(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStaticDataMember", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsStaticDataMember(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStrictlyPositive", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsStrictlyPositive(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTemplated", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsTemplated(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsThisDeclarationADefinition", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsThisDeclarationADefinition(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTransparentTag", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsTransparentTag(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTypeConcept", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsTypeConcept(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnavailable", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsUnavailable(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnnamedBitfield", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsUnnamedBitfield(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnsigned", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsUnsigned(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnsupportedFriend", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsUnsupportedFriend(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsVariadic", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getIsVariadic(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaCallOperator", ExactSpelling = true)]
        public static extern CXCursor Cursor_getLambdaCallOperator(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaContextDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getLambdaContextDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaStaticInvoker", ExactSpelling = true)]
        public static extern CXCursor Cursor_getLambdaStaticInvoker(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLhsExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getLhsExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMaxAlignment", ExactSpelling = true)]
        [return: NativeTypeName("unsigned int")]
        public static extern uint Cursor_getMaxAlignment(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMostRecentDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getMostRecentDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMostRecentNonInjectedDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getMostRecentNonInjectedDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNextDeclInContext", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNextDeclInContext(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNextSwitchCase", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNextSwitchCase(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNominatedBaseClass", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNominatedBaseClass(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNominatedBaseClassShadowDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNominatedBaseClassShadowDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNonClosureContext", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNonClosureContext(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumAssociatedConstraints", ExactSpelling = true)]
        public static extern int Cursor_getNumAssociatedConstraints(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumArguments", ExactSpelling = true)]
        public static extern int Cursor_getNumArguments(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumCaptures", ExactSpelling = true)]
        public static extern int Cursor_getNumCaptures(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumExprs", ExactSpelling = true)]
        public static extern int Cursor_getNumExprs(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumSpecializations", ExactSpelling = true)]
        public static extern int Cursor_getNumSpecializations(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumTemplateArguments", ExactSpelling = true)]
        public static extern int Cursor_getNumTemplateArguments(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOpaqueValueExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getOpaqueValueExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOriginalType", ExactSpelling = true)]
        public static extern CXType Cursor_getOriginalType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getParentFunctionOrMethod", ExactSpelling = true)]
        public static extern CXCursor Cursor_getParentFunctionOrMethod(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPlaceholderTypeConstraint", ExactSpelling = true)]
        public static extern CXCursor Cursor_getPlaceholderTypeConstraint(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPreviousDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getPreviousDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPrimaryTemplate", ExactSpelling = true)]
        public static extern CXCursor Cursor_getPrimaryTemplate(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReferenced", ExactSpelling = true)]
        public static extern CXCursor Cursor_getReferenced(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReturnType", ExactSpelling = true)]
        public static extern CXType Cursor_getReturnType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getRhsExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getRhsExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSourceRange", ExactSpelling = true)]
        public static extern CXSourceRange Cursor_getSourceRange(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSpecialization", ExactSpelling = true)]
        public static extern CXCursor Cursor_getSpecialization(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getStmtClass", ExactSpelling = true)]
        public static extern CX_StmtClass Cursor_getStmtClass(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getStringLiteralValue", ExactSpelling = true)]
        public static extern CXString Cursor_getStringLiteralValue(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSubExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getSubExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSubExprAsWritten", ExactSpelling = true)]
        public static extern CXCursor Cursor_getSubExprAsWritten(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSubStmt", ExactSpelling = true)]
        public static extern CXCursor Cursor_getSubStmt(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTargetUnionField", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTargetUnionField(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgument", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgument(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentAsDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentAsDecl(CXCursor C, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentAsExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentAsExpr(CXCursor C, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentAsIntegral", ExactSpelling = true)]
        public static extern long Cursor_getTemplateArgumentAsIntegral(CXCursor C, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentAsType", ExactSpelling = true)]
        public static extern CXType Cursor_getTemplateArgumentAsType(CXCursor C, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentIntegralType", ExactSpelling = true)]
        public static extern CXType Cursor_getTemplateArgumentIntegralType(CXCursor C, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentKind", ExactSpelling = true)]
        public static extern CXTemplateArgumentKind Cursor_getTemplateArgumentKind(CXCursor C, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocLocation", ExactSpelling = true)]
        public static extern CXSourceLocation Cursor_getTemplateArgumentLocLocation(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocSourceDeclExpression", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentLocSourceDeclExpression(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocSourceExpression", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentLocSourceExpression(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocSourceIntegralExpression", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentLocSourceIntegralExpression(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocSourceNullPtrExpression", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentLocSourceNullPtrExpression(CXCursor C, [NativeTypeName("unsigned int")] uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentNullPtrType", ExactSpelling = true)]
        public static extern CXType Cursor_getTemplateArgumentNullPtrType(CXCursor C, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplatedDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplatedDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateInstantiationPattern", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateInstantiationPattern(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateSpecializationKind", ExactSpelling = true)]
        public static extern CX_TemplateSpecializationKind Cursor_getTemplateSpecializationKind(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateTypeParmDepth", ExactSpelling = true)]
        public static extern int Cursor_getTemplateTypeParmDepth(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateTypeParmIndex", ExactSpelling = true)]
        public static extern int Cursor_getTemplateTypeParmIndex(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getThisObjectType", ExactSpelling = true)]
        public static extern CXType Cursor_getThisObjectType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getThisType", ExactSpelling = true)]
        public static extern CXType Cursor_getThisType(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTrailingRequiresClause", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTrailingRequiresClause(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTrueExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTrueExpr(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTypedefNameForAnonDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTypedefNameForAnonDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnaryExprOrTypeTraitKind", ExactSpelling = true)]
        public static extern CX_UnaryExprOrTypeTrait Cursor_getUnaryExprOrTypeTraitKind(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnaryOpcode", ExactSpelling = true)]
        public static extern CX_UnaryOperatorKind Cursor_getUnaryOpcode(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnaryOpcodeSpelling", ExactSpelling = true)]
        public static extern CXString Cursor_getUnaryOpcodeSpelling(CX_UnaryOperatorKind Op);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnderlyingDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getUnderlyingDecl(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUninstantiatedDefaultArg", ExactSpelling = true)]
        public static extern CXCursor Cursor_getUninstantiatedDefaultArg(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUsedContext", ExactSpelling = true)]
        public static extern CXCursor Cursor_getUsedContext(CXCursor C);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_desugar", ExactSpelling = true)]
        public static extern CXType Type_desugar(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getAddrSpaceExpr", ExactSpelling = true)]
        public static extern CXCursor Type_getAddrSpaceExpr(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getAdjustedType", ExactSpelling = true)]
        public static extern CXType Type_getAdjustedType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getAdjustedType", ExactSpelling = true)]
        public static extern CX_AttrKind Type_getAttrKind(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getBaseType", ExactSpelling = true)]
        public static extern CXType Type_getBaseType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDecayedType", ExactSpelling = true)]
        public static extern CXType Type_getDecayedType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDeclaration", ExactSpelling = true)]
        public static extern CXCursor Type_getDeclaration(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDeducedType", ExactSpelling = true)]
        public static extern CXType Type_getDeducedType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDepth", ExactSpelling = true)]
        public static extern int Type_getDepth(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getElementType", ExactSpelling = true)]
        public static extern CXType Type_getElementType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getEquivalentType", ExactSpelling = true)]
        public static extern CXType Type_getEquivalentType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIndex", ExactSpelling = true)]
        public static extern int Type_getIndex(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getInjectedSpecializationType", ExactSpelling = true)]
        public static extern CXType Type_getInjectedSpecializationType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getInjectedTST", ExactSpelling = true)]
        public static extern CXType Type_getInjectedTST(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsSugared", ExactSpelling = true)]
        public static extern uint Type_getIsSugared(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsTypeAlias", ExactSpelling = true)]
        public static extern uint Type_getIsTypeAlias(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getModifiedType", ExactSpelling = true)]
        public static extern CXType Type_getModifiedType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getOriginalType", ExactSpelling = true)]
        public static extern CXType Type_getOriginalType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getOwnedTagDecl", ExactSpelling = true)]
        public static extern CXCursor Type_getOwnedTagDecl(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getPointeeType", ExactSpelling = true)]
        public static extern CXType Type_getPointeeType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getSizeExpr", ExactSpelling = true)]
        public static extern CXCursor Type_getSizeExpr(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateArgumentAsDecl", ExactSpelling = true)]
        public static extern CXCursor Type_getTemplateArgumentAsDecl(CXType CT, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateArgumentAsExpr", ExactSpelling = true)]
        public static extern CXCursor Type_getTemplateArgumentAsExpr(CXType CT, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateArgumentAsIntegral", ExactSpelling = true)]
        public static extern long Type_getTemplateArgumentAsIntegral(CXType CT, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateArgumentAsType", ExactSpelling = true)]
        public static extern CXType Type_getTemplateArgumentAsType(CXType CT, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateArgumentIntegralType", ExactSpelling = true)]
        public static extern CXType Type_getTemplateArgumentIntegralType(CXType CT, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateArgumentKind", ExactSpelling = true)]
        public static extern CXTemplateArgumentKind Type_getTemplateArgumentKind(CXType CT, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateArgumentNullPtrType", ExactSpelling = true)]
        public static extern CXType Type_getTemplateArgumentNullPtrType(CXType CT, uint i);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTypeClass", ExactSpelling = true)]
        public static extern CX_TypeClass Type_getTypeClass(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getUnderlyingExpr", ExactSpelling = true)]
        public static extern CXCursor Type_getUnderlyingExpr(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getUnderlyingType", ExactSpelling = true)]
        public static extern CXType Type_getUnderlyingType(CXType CT);

        [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getValueType", ExactSpelling = true)]
        public static extern CXType Type_getValueType(CXType CT);
    }
}
