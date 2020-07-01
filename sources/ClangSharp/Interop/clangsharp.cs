// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/master/sources/libClangSharp

using System.Runtime.InteropServices;

namespace ClangSharp.Interop
{
    public static unsafe partial class clangsharp
    {
        private const string LibraryPath = "libClangSharp";

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAssociatedConstraint", ExactSpelling = true)]
        public static extern CXCursor Cursor_getAssociatedConstraint(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getArgument", ExactSpelling = true)]
        public static extern CXCursor Cursor_getArgument(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAsFunction", ExactSpelling = true)]
        public static extern CXCursor Cursor_getAsFunction(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAttrKind", ExactSpelling = true)]
        public static extern CX_AttrKind Cursor_getAttrKind(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinaryOpcode", ExactSpelling = true)]
        public static extern CX_BinaryOperatorKind Cursor_getBinaryOpcode(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinaryOpcodeSpelling", ExactSpelling = true)]
        public static extern CXString Cursor_getBinaryOpcodeSpelling(CX_BinaryOperatorKind Op);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinding", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBinding(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBitWidth", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBitWidth(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBlockManglingContextDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBlockManglingContextDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBlockManglingNumber", ExactSpelling = true)]
        public static extern int Cursor_getBlockManglingNumber(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBlockMissingReturnType", ExactSpelling = true)]
        public static extern uint Cursor_getBlockMissingReturnType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBody", ExactSpelling = true)]
        public static extern CXCursor Cursor_getBody(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCallResultType", ExactSpelling = true)]
        public static extern CXType Cursor_getCallResultType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCanAvoidCopyToHeap", ExactSpelling = true)]
        public static extern uint Cursor_getCanAvoidCopyToHeap(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureCopyExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getCaptureCopyExpr(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureHasCopyExpr", ExactSpelling = true)]
        public static extern uint Cursor_getCaptureHasCopyExpr(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureIsByRef", ExactSpelling = true)]
        public static extern uint Cursor_getCaptureIsByRef(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureIsEscapingByRef", ExactSpelling = true)]
        public static extern uint Cursor_getCaptureIsEscapingByRef(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureIsNested", ExactSpelling = true)]
        public static extern uint Cursor_getCaptureIsNested(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureIsNonEscapingByRef", ExactSpelling = true)]
        public static extern uint Cursor_getCaptureIsNonEscapingByRef(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturesCXXThis", ExactSpelling = true)]
        public static extern uint Cursor_getCapturesCXXThis(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturesVariable", ExactSpelling = true)]
        public static extern uint Cursor_getCapturesVariable(CXCursor C, CXCursor V);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureVariable", ExactSpelling = true)]
        public static extern CXCursor Cursor_getCaptureVariable(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCastKind", ExactSpelling = true)]
        public static extern CX_CastKind Cursor_getCastKind(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstraintExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getConstraintExpr(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructedBaseClass", ExactSpelling = true)]
        public static extern CXCursor Cursor_getConstructedBaseClass(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructedBaseClassShadowDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getConstructedBaseClassShadowDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructsVirtualBase", ExactSpelling = true)]
        public static extern uint Cursor_getConstructsVirtualBase(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getContextParam", ExactSpelling = true)]
        public static extern CXCursor Cursor_getContextParam(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getContextParamPosition", ExactSpelling = true)]
        public static extern int Cursor_getContextParamPosition(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDeclaredReturnType", ExactSpelling = true)]
        public static extern CXType Cursor_getDeclaredReturnType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDeclKind", ExactSpelling = true)]
        public static extern CX_DeclKind Cursor_getDeclKind(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDecomposedDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDecomposedDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDefaultArg", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDefaultArg(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDefaultArgType", ExactSpelling = true)]
        public static extern CXType Cursor_getDefaultArgType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDependentLambdaCallOperator", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDependentLambdaCallOperator(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDescribedClassTemplate", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDescribedClassTemplate(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDescribedTemplate", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDescribedTemplate(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDestructor", ExactSpelling = true)]
        public static extern CXCursor Cursor_getDestructor(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDoesNotEscape", ExactSpelling = true)]
        public static extern uint Cursor_getDoesNotEscape(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getEnumDeclPromotionType", ExactSpelling = true)]
        public static extern CXType Cursor_getEnumDeclPromotionType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFieldIndex", ExactSpelling = true)]
        public static extern int Cursor_getFieldIndex(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFriendDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getFriendDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFunctionScopeDepth", ExactSpelling = true)]
        public static extern int Cursor_getFunctionScopeDepth(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFunctionScopeIndex", ExactSpelling = true)]
        public static extern int Cursor_getFunctionScopeIndex(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasBody", ExactSpelling = true)]
        public static extern uint Cursor_getHasBody(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasDefaultArg", ExactSpelling = true)]
        public static extern uint Cursor_getHasDefaultArg(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasExplicitTemplateArgs", ExactSpelling = true)]
        public static extern uint Cursor_getHasExplicitTemplateArgs(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasExternalStorage", ExactSpelling = true)]
        public static extern uint Cursor_getHasExternalStorage(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasGlobalStorage", ExactSpelling = true)]
        public static extern uint Cursor_getHasGlobalStorage(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasImplicitReturnZero", ExactSpelling = true)]
        public static extern uint Cursor_getHasImplicitReturnZero(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasInheritedDefaultArg", ExactSpelling = true)]
        public static extern uint Cursor_getHasInheritedDefaultArg(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasInit", ExactSpelling = true)]
        public static extern uint Cursor_getHasInit(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasLocalStorage", ExactSpelling = true)]
        public static extern uint Cursor_getHasLocalStorage(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasPlaceholderTypeConstraint", ExactSpelling = true)]
        public static extern uint Cursor_getHasPlaceholderTypeConstraint(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHoldingVar", ExactSpelling = true)]
        public static extern CXCursor Cursor_getHoldingVar(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInClassInitializer", ExactSpelling = true)]
        public static extern CXCursor Cursor_getInClassInitializer(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInitExpr", ExactSpelling = true)]
        public static extern CXCursor Cursor_getInitExpr(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInstantiatedFromMember", ExactSpelling = true)]
        public static extern CXCursor Cursor_getInstantiatedFromMember(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsAnonymousStructOrUnion", ExactSpelling = true)]
        public static extern uint Cursor_getIsAnonymousStructOrUnion(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsConversionFromLambda", ExactSpelling = true)]
        public static extern uint Cursor_getIsConversionFromLambda(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDefined", ExactSpelling = true)]
        public static extern uint Cursor_getIsDefined(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDeprecated", ExactSpelling = true)]
        public static extern uint Cursor_getIsDeprecated(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsExternC", ExactSpelling = true)]
        public static extern uint Cursor_getIsExternC(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsExpandedParameterPack", ExactSpelling = true)]
        public static extern uint Cursor_getIsExpandedParameterPack(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsGlobal", ExactSpelling = true)]
        public static extern uint Cursor_getIsGlobal(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsLocalVarDecl", ExactSpelling = true)]
        public static extern uint Cursor_getIsLocalVarDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsLocalVarDeclOrParm", ExactSpelling = true)]
        public static extern uint Cursor_getIsLocalVarDeclOrParm(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsMemberSpecialization", ExactSpelling = true)]
        public static extern uint Cursor_getIsMemberSpecialization(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNegative", ExactSpelling = true)]
        public static extern uint Cursor_getIsNegative(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNonNegative", ExactSpelling = true)]
        public static extern uint Cursor_getIsNonNegative(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNoReturn", ExactSpelling = true)]
        public static extern uint Cursor_getIsNoReturn(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNothrow", ExactSpelling = true)]
        public static extern uint Cursor_getIsNothrow(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsOverloadedOperator", ExactSpelling = true)]
        public static extern uint Cursor_getIsOverloadedOperator(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPackExpansion", ExactSpelling = true)]
        public static extern uint Cursor_getIsPackExpansion(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsParameterPack", ExactSpelling = true)]
        public static extern uint Cursor_getIsParameterPack(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPure", ExactSpelling = true)]
        public static extern uint Cursor_getIsPure(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsSigned", ExactSpelling = true)]
        public static extern uint Cursor_getIsSigned(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStatic", ExactSpelling = true)]
        public static extern uint Cursor_getIsStatic(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStaticDataMember", ExactSpelling = true)]
        public static extern uint Cursor_getIsStaticDataMember(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStrictlyPositive", ExactSpelling = true)]
        public static extern uint Cursor_getIsStrictlyPositive(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTemplated", ExactSpelling = true)]
        public static extern uint Cursor_getIsTemplated(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsThisDeclarationADefinition", ExactSpelling = true)]
        public static extern uint Cursor_getIsThisDeclarationADefinition(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTransparentTag", ExactSpelling = true)]
        public static extern uint Cursor_getIsTransparentTag(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTypeConcept", ExactSpelling = true)]
        public static extern uint Cursor_getIsTypeConcept(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnavailable", ExactSpelling = true)]
        public static extern uint Cursor_getIsUnavailable(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnnamedBitfield", ExactSpelling = true)]
        public static extern uint Cursor_getIsUnnamedBitfield(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnsigned", ExactSpelling = true)]
        public static extern uint Cursor_getIsUnsigned(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnsupportedFriend", ExactSpelling = true)]
        public static extern uint Cursor_getIsUnsupportedFriend(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsVariadic", ExactSpelling = true)]
        public static extern uint Cursor_getIsVariadic(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaCallOperator", ExactSpelling = true)]
        public static extern CXCursor Cursor_getLambdaCallOperator(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaContextDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getLambdaContextDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaStaticInvoker", ExactSpelling = true)]
        public static extern CXCursor Cursor_getLambdaStaticInvoker(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMaxAlignment", ExactSpelling = true)]
        public static extern uint Cursor_getMaxAlignment(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMostRecentDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getMostRecentDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMostRecentNonInjectedDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getMostRecentNonInjectedDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNextDeclInContext", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNextDeclInContext(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNominatedBaseClass", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNominatedBaseClass(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNominatedBaseClassShadowDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNominatedBaseClassShadowDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNonClosureContext", ExactSpelling = true)]
        public static extern CXCursor Cursor_getNonClosureContext(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumAssociatedConstraints", ExactSpelling = true)]
        public static extern int Cursor_getNumAssociatedConstraints(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumArguments", ExactSpelling = true)]
        public static extern int Cursor_getNumArguments(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumCaptures", ExactSpelling = true)]
        public static extern int Cursor_getNumCaptures(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumSpecializations", ExactSpelling = true)]
        public static extern int Cursor_getNumSpecializations(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumTemplateArguments", ExactSpelling = true)]
        public static extern int Cursor_getNumTemplateArguments(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOriginalType", ExactSpelling = true)]
        public static extern CXType Cursor_getOriginalType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getParentFunctionOrMethod", ExactSpelling = true)]
        public static extern CXCursor Cursor_getParentFunctionOrMethod(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPlaceholderTypeConstraint", ExactSpelling = true)]
        public static extern CXCursor Cursor_getPlaceholderTypeConstraint(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPreviousDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getPreviousDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPrimaryTemplate", ExactSpelling = true)]
        public static extern CXCursor Cursor_getPrimaryTemplate(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReturnType", ExactSpelling = true)]
        public static extern CXType Cursor_getReturnType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSourceRange", ExactSpelling = true)]
        public static extern CXSourceRange Cursor_getSourceRange(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSpecialization", ExactSpelling = true)]
        public static extern CXCursor Cursor_getSpecialization(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getStmtClass", ExactSpelling = true)]
        public static extern CX_StmtClass Cursor_getStmtClass(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgument", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgument(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocLocation", ExactSpelling = true)]
        public static extern CXSourceLocation Cursor_getTemplateArgumentLocLocation(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocSourceDeclExpression", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentLocSourceDeclExpression(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocSourceExpression", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentLocSourceExpression(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocSourceIntegralExpression", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentLocSourceIntegralExpression(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLocSourceNullPtrExpression", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateArgumentLocSourceNullPtrExpression(CXCursor C, uint i);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplatedDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplatedDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateInstantiationPattern", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTemplateInstantiationPattern(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateSpecializationKind", ExactSpelling = true)]
        public static extern CX_TemplateSpecializationKind Cursor_getTemplateSpecializationKind(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateTypeParmDepth", ExactSpelling = true)]
        public static extern int Cursor_getTemplateTypeParmDepth(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateTypeParmIndex", ExactSpelling = true)]
        public static extern int Cursor_getTemplateTypeParmIndex(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getThisObjectType", ExactSpelling = true)]
        public static extern CXType Cursor_getThisObjectType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getThisType", ExactSpelling = true)]
        public static extern CXType Cursor_getThisType(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTrailingRequiresClause", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTrailingRequiresClause(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTypedefNameForAnonDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getTypedefNameForAnonDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnaryOpcode", ExactSpelling = true)]
        public static extern CX_UnaryOperatorKind Cursor_getUnaryOpcode(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnaryOpcodeSpelling", ExactSpelling = true)]
        public static extern CXString Cursor_getUnaryOpcodeSpelling(CX_UnaryOperatorKind Op);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnderlyingDecl", ExactSpelling = true)]
        public static extern CXCursor Cursor_getUnderlyingDecl(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUninstantiatedDefaultArg", ExactSpelling = true)]
        public static extern CXCursor Cursor_getUninstantiatedDefaultArg(CXCursor C);

        [DllImport(LibraryPath, CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTypeClass", ExactSpelling = true)]
        public static extern CX_TypeClass Type_getTypeClass(CXType CT);
    }
}
