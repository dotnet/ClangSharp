// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

using System;
using System.Runtime.InteropServices;

namespace ClangSharp.Interop;

public static unsafe partial class @clangsharp
{
    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getArgument", ExactSpelling = true)]
    public static extern CXCursor Cursor_getArgument(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAllocate", ExactSpelling = true)]
    public static extern CXCursor Cursor_getAllocate(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAsmString", ExactSpelling = true)]
    public static extern CXString Cursor_getAsmString(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAsmStringExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getAsmStringExpr(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getArgumentType", ExactSpelling = true)]
    public static extern CXType Cursor_getArgumentType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getArraySize", ExactSpelling = true)]
    [return: NativeTypeName("int64_t")]
    public static extern long Cursor_getArraySize(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAssociatedConstraint", ExactSpelling = true)]
    public static extern CXCursor Cursor_getAssociatedConstraint(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAsFunction", ExactSpelling = true)]
    public static extern CXCursor Cursor_getAsFunction(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAtomicOpcode", ExactSpelling = true)]
    public static extern CX_AtomicOperatorKind Cursor_getAtomicOpcode(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAttr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getAttr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAttrKind", ExactSpelling = true)]
    public static extern CX_AttrKind Cursor_getAttrKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAttrSpelling", ExactSpelling = true)]
    public static extern CXString Cursor_getAttrSpelling(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAvailabilityAttributeDeprecated", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getAvailabilityAttributeDeprecated(CXCursor param0, [NativeTypeName("llvm::VersionTuple *")] VersionTuple* version);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAvailabilityAttributeIntroduced", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getAvailabilityAttributeIntroduced(CXCursor param0, [NativeTypeName("llvm::VersionTuple *")] VersionTuple* version);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAvailabilityAttributeMessage", ExactSpelling = true)]
    public static extern CXString Cursor_getAvailabilityAttributeMessage(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAvailabilityAttributeObsoleted", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getAvailabilityAttributeObsoleted(CXCursor param0, [NativeTypeName("llvm::VersionTuple *")] VersionTuple* version);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAvailabilityAttributePlatformIdentifierName", ExactSpelling = true)]
    public static extern CXString Cursor_getAvailabilityAttributePlatformIdentifierName(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getAvailabilityAttributeUnavailable", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getAvailabilityAttributeUnavailable(CXCursor param0);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBase", ExactSpelling = true)]
    public static extern CXCursor Cursor_getBase(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBinaryOpcode", ExactSpelling = true)]
    public static extern CXBinaryOperatorKind Cursor_getBinaryOpcode(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBindingDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getBindingDecl(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBindingExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getBindingExpr(CXCursor C);

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

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCallResultType", ExactSpelling = true)]
    public static extern CXType Cursor_getCallResultType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getClobber", ExactSpelling = true)]
    public static extern CXString Cursor_getClobber(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCanAvoidCopyToHeap", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getCanAvoidCopyToHeap(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCanonical", ExactSpelling = true)]
    public static extern CXCursor Cursor_getCanonical(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureCopyExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getCaptureCopyExpr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturedVar", ExactSpelling = true)]
    public static extern CXCursor Cursor_getCapturedVar(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCaptureKind", ExactSpelling = true)]
    public static extern CX_VariableCaptureKind Cursor_getCaptureKind(CXCursor C, [NativeTypeName("unsigned int")] uint i);

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

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturedDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getCapturedDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturedRecordDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getCapturedRecordDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturedRegionKind", ExactSpelling = true)]
    public static extern CX_CapturedRegionKind Cursor_getCapturedRegionKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCapturedStmt", ExactSpelling = true)]
    public static extern CXCursor Cursor_getCapturedStmt(CXCursor C);

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

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getChild", ExactSpelling = true)]
    public static extern CXCursor Cursor_getChild(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getComputationLhsType", ExactSpelling = true)]
    public static extern CXType Cursor_getComputationLhsType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getComputationResultType", ExactSpelling = true)]
    public static extern CXType Cursor_getComputationResultType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstraintExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getConstraintExpr(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructedBaseClass", ExactSpelling = true)]
    public static extern CXCursor Cursor_getConstructedBaseClass(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructedBaseClassShadowDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getConstructedBaseClassShadowDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructionKind", ExactSpelling = true)]
    public static extern CX_ConstructionKind Cursor_getConstructionKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstexprKind", ExactSpelling = true)]
    public static extern CX_ConstexprSpecKind Cursor_getConstexprKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getConstructsVirtualBase", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getConstructsVirtualBase(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getContextParam", ExactSpelling = true)]
    public static extern CXCursor Cursor_getContextParam(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getContextParamPosition", ExactSpelling = true)]
    public static extern int Cursor_getContextParamPosition(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCtor", ExactSpelling = true)]
    public static extern CXCursor Cursor_getCtor(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCXXRecord_IsPOD", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getCXXRecord_IsPOD(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getBoolLiteralValue", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getBoolLiteralValue(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getCookedLiteral", ExactSpelling = true)]
    public static extern CXCursor Cursor_getCookedLiteral(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDeclaredReturnType", ExactSpelling = true)]
    public static extern CXType Cursor_getDeclaredReturnType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDeallocate", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDeallocate(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getExceptionHandler", ExactSpelling = true)]
    public static extern CXCursor Cursor_getExceptionHandler(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFallthroughHandler", ExactSpelling = true)]
    public static extern CXCursor Cursor_getFallthroughHandler(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFinalSuspendStmt", ExactSpelling = true)]
    public static extern CXCursor Cursor_getFinalSuspendStmt(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDecl(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDeclKind", ExactSpelling = true)]
    public static extern CX_DeclKind Cursor_getDeclKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDecomposedDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDecomposedDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDefaultArg", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDefaultArg(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDefaultArgType", ExactSpelling = true)]
    public static extern CXType Cursor_getDefaultArgType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDefinition", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDefinition(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDependentLambdaCallOperator", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDependentLambdaCallOperator(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDescribedCursorTemplate", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDescribedCursorTemplate(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDescribedTemplate", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDescribedTemplate(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDestructor", ExactSpelling = true)]
    public static extern CXCursor Cursor_getDestructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDoesNotEscape", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getDoesNotEscape(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDoesUsualArrayDeleteWantSize", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getDoesUsualArrayDeleteWantSize(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getEnumDeclPromotionType", ExactSpelling = true)]
    public static extern CXType Cursor_getEnumDeclPromotionType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getEnumerator", ExactSpelling = true)]
    public static extern CXCursor Cursor_getEnumerator(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getExpansionType", ExactSpelling = true)]
    public static extern CXType Cursor_getExpansionType(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getExpr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getExprDependence", ExactSpelling = true)]
    public static extern CX_ExprDependence Cursor_getExprDependence(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFieldIndex", ExactSpelling = true)]
    public static extern int Cursor_getFieldIndex(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFloatingLiteralSemantics", ExactSpelling = true)]
    public static extern CX_FloatingSemantics Cursor_getFloatingLiteralSemantics(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFloatingLiteralValueAsApproximateDouble", ExactSpelling = true)]
    public static extern double Cursor_getFloatingLiteralValueAsApproximateDouble(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFoundDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getFoundDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getField", ExactSpelling = true)]
    public static extern CXCursor Cursor_getField(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFriend", ExactSpelling = true)]
    public static extern CXCursor Cursor_getFriend(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFriendDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getFriendDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFunctionScopeDepth", ExactSpelling = true)]
    public static extern int Cursor_getFunctionScopeDepth(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getFunctionScopeIndex", ExactSpelling = true)]
    public static extern int Cursor_getFunctionScopeIndex(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getGuidValue", ExactSpelling = true)]
    [return: NativeTypeName("clang::MSGuidDeclParts")]
    public static extern Guid Cursor_getGuidValue(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHadMultipleCandidates", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHadMultipleCandidates(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasAPValueResult", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasAPValueResult(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasBody", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasBody(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasBraces", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasBraces(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasDependentPromiseType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasDependentPromiseType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasDefaultArg", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasDefaultArg(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUnparsedDefaultArg", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUnparsedDefaultArg(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUninstantiatedDefaultArg", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUninstantiatedDefaultArg(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasElseStorage", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasElseStorage(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasExplicitTemplateArgs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasExplicitTemplateArgs(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasImplicitReturnZero", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasImplicitReturnZero(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasInheritedDefaultArg", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasInheritedDefaultArg(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasInit", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasInit(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasInitStorage", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasInitStorage(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasLeadingEmptyMacro", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasLeadingEmptyMacro(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasLocalStorage", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasLocalStorage(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasPlaceholderTypeConstraint", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasPlaceholderTypeConstraint(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasTemplateKeyword", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasTemplateKeyword(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUserDeclaredConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUserDeclaredConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUserDeclaredCopyAssignment", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUserDeclaredCopyAssignment(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUserDeclaredCopyConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUserDeclaredCopyConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUserDeclaredDestructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUserDeclaredDestructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUserDeclaredMoveAssignment", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUserDeclaredMoveAssignment(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUserDeclaredMoveConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUserDeclaredMoveConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasUserDeclaredMoveOperation", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasUserDeclaredMoveOperation(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasDeletedDestructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasDeletedDestructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasInClassInitializer", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasInClassInitializer(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasMutableFields", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasMutableFields(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasNonTrivialDefaultConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasNonTrivialDefaultConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasNonTrivialDestructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasNonTrivialDestructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasPrivateFields", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasPrivateFields(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasProtectedFields", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasProtectedFields(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasTrivialCopyConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasTrivialCopyConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasTrivialDefaultConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasTrivialDefaultConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasTrivialDestructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasTrivialDestructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasVarStorage", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasVarStorage(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHoldingVar", ExactSpelling = true)]
    public static extern CXCursor Cursor_getHoldingVar(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInClassInitializer", ExactSpelling = true)]
    public static extern CXCursor Cursor_getInClassInitializer(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIdentKind", ExactSpelling = true)]
    public static extern CX_PredefinedIdentKind Cursor_getIdentKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIfStatementKind", ExactSpelling = true)]
    public static extern CX_IfStatementKind Cursor_getIfStatementKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIgnoreParenNoopCasts", ExactSpelling = true)]
    public static extern CXCursor Cursor_getIgnoreParenNoopCasts(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInheritedConstructor", ExactSpelling = true)]
    public static extern CXCursor Cursor_getInheritedConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInheritedFromVBase", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getInheritedFromVBase(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInitExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getInitExpr(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInitSuspendStmt", ExactSpelling = true)]
    public static extern CXCursor Cursor_getInitSuspendStmt(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInputConstraint", ExactSpelling = true)]
    public static extern CXString Cursor_getInputConstraint(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInputExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getInputExpr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInputName", ExactSpelling = true)]
    public static extern CXString Cursor_getInputName(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInitStyle", ExactSpelling = true)]
    public static extern CX_InitializationStyle Cursor_getInitStyle(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInjectedSpecializationType", ExactSpelling = true)]
    public static extern CXType Cursor_getInjectedSpecializationType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInstantiatedFromMember", ExactSpelling = true)]
    public static extern CXCursor Cursor_getInstantiatedFromMember(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIntegerLiteralValue", ExactSpelling = true)]
    [return: NativeTypeName("int64_t")]
    public static extern long Cursor_getIntegerLiteralValue(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnsignedIntegerLiteralValue", ExactSpelling = true)]
    [return: NativeTypeName("uint64_t")]
    public static extern ulong Cursor_getUnsignedIntegerLiteralValue(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsAllEnumCasesCovered", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsAllEnumCasesCovered(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsAlwaysNull", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsAlwaysNull(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsAnonymousStructOrUnion", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsAnonymousStructOrUnion(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsArgumentType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsArgumentType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsArrayForm", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsArrayForm(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsArrayFormAsWritten", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsArrayFormAsWritten(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsArrow", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsArrow(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsCBuffer", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsCBuffer(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsClassExtension", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsClassExtension(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsCompleteDefinition", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsCompleteDefinition(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsConditionTrue", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsConditionTrue(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsConstexpr", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsConstexpr(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsConversionFromLambda", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsConversionFromLambda(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsCopyOrMoveConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsCopyOrMoveConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsCXXTry", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsCXXTry(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDefined", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsDefined(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDelegatingConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsDelegatingConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDeleted", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsDeleted(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDeprecated", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsDeprecated(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsElidable", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsElidable(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsExpandedParameterPack", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsExpandedParameterPack(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsExplicitlyDefaulted", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsExplicitlyDefaulted(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsExternC", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsExternC(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsFileScope", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsFileScope(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsFixed", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsFixed(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsGlobal", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsGlobal(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsInjectedClassName", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsInjectedClassName(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsIfExists", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsIfExists(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsImplicit", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsImplicit(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsAsmGoto", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsAsmGoto(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsSimple", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsSimple(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsVolatile", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsVolatile(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsIncomplete", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsIncomplete(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsInherited", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsInherited(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsInheritingConstructor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsInheritingConstructor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsAggregate", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsAggregate(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsCapturelessLambda", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsCapturelessLambda(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsCXX11StandardLayout", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsCXX11StandardLayout(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDynamicClass", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsDynamicClass(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsEffectivelyFinal", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsEffectivelyFinal(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsEmpty", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsEmpty(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsGenericLambda", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsGenericLambda(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsLambda", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsLambda(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsLiteral", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsLiteral(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPolymorphic", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsPolymorphic(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStandardLayout", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsStandardLayout(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTrivial", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsTrivial(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTriviallyCopyable", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsTriviallyCopyable(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsLateParsed", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsLateParsed(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsListInitialization", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsListInitialization(CXCursor C);

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

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsNested", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsNested(CXCursor C);

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

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPartiallySubstituted", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsPartiallySubstituted(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPotentiallyEvaluated", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsPotentiallyEvaluated(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPureVirtual", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsPureVirtual(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsResultDependent", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsResultDependent(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsReversed", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsReversed(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsSigned", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsSigned(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStatic", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsStatic(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStaticDataMember", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsStaticDataMember(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStdInitListInitialization", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsStdInitListInitialization(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsStrictlyPositive", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsStrictlyPositive(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTemplated", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsTemplated(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsThisDeclarationADefinition", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsThisDeclarationADefinition(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsPropertyAccessor", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsPropertyAccessor(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsThrownVariableInScope", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsThrownVariableInScope(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTransparent", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsTransparent(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsTypeConcept", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsTypeConcept(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnavailable", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsUnavailable(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnconditionallyVisible", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsUnconditionallyVisible(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnnamedBitfield", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsUnnamedBitfield(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnsigned", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsUnsigned(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUnsupportedFriend", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsUnsupportedFriend(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsUserProvided", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsUserProvided(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsVariadic", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsVariadic(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsZeroLengthBitField", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsZeroLengthBitField(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaCallOperator", ExactSpelling = true)]
    public static extern CXCursor Cursor_getLambdaCallOperator(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaCaptureDefault", ExactSpelling = true)]
    public static extern CX_LambdaCaptureDefault Cursor_getLambdaCaptureDefault(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaContextDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getLambdaContextDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLambdaStaticInvoker", ExactSpelling = true)]
    public static extern CXCursor Cursor_getLambdaStaticInvoker(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLabelExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getLabelExpr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLabelName", ExactSpelling = true)]
    public static extern CXString Cursor_getLabelName(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLhsExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getLhsExpr(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getLiteralOperatorKind", ExactSpelling = true)]
    public static extern CX_LiteralOperatorKind Cursor_getLiteralOperatorKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMaxAlignment", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getMaxAlignment(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMethod", ExactSpelling = true)]
    public static extern CXCursor Cursor_getMethod(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMethodFamily", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getMethodFamily(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMostRecentDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getMostRecentDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getName", ExactSpelling = true)]
    public static extern CXString Cursor_getName(CXCursor C);

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

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumArguments", ExactSpelling = true)]
    public static extern int Cursor_getNumArguments(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumClobbers", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getNumClobbers(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumInputs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getNumInputs(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumLabels", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getNumLabels(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumOutputs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getNumOutputs(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumAssocs", ExactSpelling = true)]
    public static extern int Cursor_getNumAssocs(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumAssociatedConstraints", ExactSpelling = true)]
    public static extern int Cursor_getNumAssociatedConstraints(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumAttrs", ExactSpelling = true)]
    public static extern int Cursor_getNumAttrs(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumBases", ExactSpelling = true)]
    public static extern int Cursor_getNumBases(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumBindings", ExactSpelling = true)]
    public static extern int Cursor_getNumBindings(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumCaptures", ExactSpelling = true)]
    public static extern int Cursor_getNumCaptures(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumChildren", ExactSpelling = true)]
    public static extern int Cursor_getNumChildren(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumCtors", ExactSpelling = true)]
    public static extern int Cursor_getNumCtors(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumDecls", ExactSpelling = true)]
    public static extern int Cursor_getNumDecls(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumEnumerators", ExactSpelling = true)]
    public static extern int Cursor_getNumEnumerators(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumExpansionTypes", ExactSpelling = true)]
    public static extern int Cursor_getNumExpansionTypes(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumExprs", ExactSpelling = true)]
    public static extern int Cursor_getNumExprs(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumExprsOther", ExactSpelling = true)]
    public static extern int Cursor_getNumExprsOther(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumFields", ExactSpelling = true)]
    public static extern int Cursor_getNumFields(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumFriends", ExactSpelling = true)]
    public static extern int Cursor_getNumFriends(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumMethods", ExactSpelling = true)]
    public static extern int Cursor_getNumMethods(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumProtocols", ExactSpelling = true)]
    public static extern int Cursor_getNumProtocols(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumSpecializations", ExactSpelling = true)]
    public static extern int Cursor_getNumSpecializations(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumTemplateArguments", ExactSpelling = true)]
    public static extern int Cursor_getNumTemplateArguments(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumTemplateParameters", ExactSpelling = true)]
    public static extern int Cursor_getNumTemplateParameters(CXCursor C, [NativeTypeName("unsigned int")] uint listIndex);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumTemplateParameterLists", ExactSpelling = true)]
    public static extern int Cursor_getNumTemplateParameterLists(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumTypeParams", ExactSpelling = true)]
    public static extern int Cursor_getNumTypeParams(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumVBases", ExactSpelling = true)]
    public static extern int Cursor_getNumVBases(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjCRuntimeNameAttrMetadataName", ExactSpelling = true)]
    public static extern CXString Cursor_getObjCRuntimeNameAttrMetadataName(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjectKind", ExactSpelling = true)]
    public static extern CX_ExprObjectKind Cursor_getObjectKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOpaqueValue", ExactSpelling = true)]
    public static extern CXCursor Cursor_getOpaqueValue(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOperand", ExactSpelling = true)]
    public static extern CXCursor Cursor_getOperand(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOutputConstraint", ExactSpelling = true)]
    public static extern CXString Cursor_getOutputConstraint(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOutputExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getOutputExpr(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOutputName", ExactSpelling = true)]
    public static extern CXString Cursor_getOutputName(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOriginalType", ExactSpelling = true)]
    public static extern CXType Cursor_getOriginalType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getOverloadedOperatorKind", ExactSpelling = true)]
    public static extern CX_OverloadedOperatorKind Cursor_getOverloadedOperatorKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPackLength", ExactSpelling = true)]
    public static extern int Cursor_getPackLength(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getParentFunctionOrMethod", ExactSpelling = true)]
    public static extern CXCursor Cursor_getParentFunctionOrMethod(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPromiseCall", ExactSpelling = true)]
    public static extern CXCursor Cursor_getPromiseCall(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPromiseDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getPromiseDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPlaceholderTypeConstraint", ExactSpelling = true)]
    public static extern CXCursor Cursor_getPlaceholderTypeConstraint(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPreviousDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getPreviousDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPrimaryTemplate", ExactSpelling = true)]
    public static extern CXCursor Cursor_getPrimaryTemplate(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getPropertyAttributes", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getPropertyAttributes(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getProtocol", ExactSpelling = true)]
    public static extern CXCursor Cursor_getProtocol(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getQualifiedName", ExactSpelling = true)]
    public static extern CXString Cursor_getQualifiedName(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getRedeclContext", ExactSpelling = true)]
    public static extern CXCursor Cursor_getRedeclContext(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReferenced", ExactSpelling = true)]
    public static extern CXCursor Cursor_getReferenced(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getRefQualifier", ExactSpelling = true)]
    public static extern CXRefQualifierKind Cursor_getRefQualifier(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getRequiresZeroInitialization", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getRequiresZeroInitialization(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getResultAsAPSInt", ExactSpelling = true)]
    [return: NativeTypeName("int64_t")]
    public static extern long Cursor_getResultAsAPSInt(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getResultIndex", ExactSpelling = true)]
    public static extern int Cursor_getResultIndex(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getResultDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getResultDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReturnStmt", ExactSpelling = true)]
    public static extern CXCursor Cursor_getReturnStmt(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReturnStmtOnAllocFailure", ExactSpelling = true)]
    public static extern CXCursor Cursor_getReturnStmtOnAllocFailure(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReturnValue", ExactSpelling = true)]
    public static extern CXCursor Cursor_getReturnValue(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReturnValueInit", ExactSpelling = true)]
    public static extern CXCursor Cursor_getReturnValueInit(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getReturnType", ExactSpelling = true)]
    public static extern CXType Cursor_getReturnType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getRhsExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getRhsExpr(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSelector", ExactSpelling = true)]
    public static extern CXString Cursor_getSelector(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getShouldCopy", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getShouldCopy(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSourceRange", ExactSpelling = true)]
    public static extern CXSourceRange Cursor_getSourceRange(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSourceRangeRaw", ExactSpelling = true)]
    public static extern CXSourceRange Cursor_getSourceRangeRaw(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSpecialization", ExactSpelling = true)]
    public static extern CXCursor Cursor_getSpecialization(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getStmtClass", ExactSpelling = true)]
    public static extern CX_StmtClass Cursor_getStmtClass(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getStringLiteralKind", ExactSpelling = true)]
    public static extern CX_StringKind Cursor_getStringLiteralKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getStringLiteralValue", ExactSpelling = true)]
    public static extern CXString Cursor_getStringLiteralValue(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSubDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getSubDecl(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSubExpr", ExactSpelling = true)]
    public static extern CXCursor Cursor_getSubExpr(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSubExprAsWritten", ExactSpelling = true)]
    public static extern CXCursor Cursor_getSubExprAsWritten(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSubStmt", ExactSpelling = true)]
    public static extern CXCursor Cursor_getSubStmt(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTargetUnionField", ExactSpelling = true)]
    public static extern CXCursor Cursor_getTargetUnionField(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgument", ExactSpelling = true)]
    public static extern CX_TemplateArgument Cursor_getTemplateArgument(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateArgumentLoc", ExactSpelling = true)]
    public static extern CX_TemplateArgumentLoc Cursor_getTemplateArgumentLoc(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateParameter", ExactSpelling = true)]
    public static extern CXCursor Cursor_getTemplateParameter(CXCursor C, [NativeTypeName("unsigned int")] uint listIndex, [NativeTypeName("unsigned int")] uint parameterIndex);

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

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTemplateTypeParmPosition", ExactSpelling = true)]
    public static extern int Cursor_getTemplateTypeParmPosition(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getThisObjectType", ExactSpelling = true)]
    public static extern CXType Cursor_getThisObjectType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getThisType", ExactSpelling = true)]
    public static extern CXType Cursor_getThisType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTrailingRequiresClause", ExactSpelling = true)]
    public static extern CXCursor Cursor_getTrailingRequiresClause(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTypedefNameForAnonDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getTypedefNameForAnonDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInclusionDirectiveKind", ExactSpelling = true)]
    public static extern CX_InclusionDirectiveKind Cursor_getInclusionDirectiveKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInclusionDirectiveImportedModule", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getInclusionDirectiveImportedModule(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getInclusionDirectiveWasInQuotes", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getInclusionDirectiveWasInQuotes(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsMacroC99Varargs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsMacroC99Varargs(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsMacroGNUVarargs", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsMacroGNUVarargs(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsMacroVariadic", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsMacroVariadic(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMacroExpansionDefinition", ExactSpelling = true)]
    public static extern CXCursor Cursor_getMacroExpansionDefinition(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMacroParamName", ExactSpelling = true)]
    public static extern CXString Cursor_getMacroParamName(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMacroTokenKind", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getMacroTokenKind(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getMacroTokenSpelling", ExactSpelling = true)]
    public static extern CXString Cursor_getMacroTokenSpelling(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumMacroParams", ExactSpelling = true)]
    public static extern int Cursor_getNumMacroParams(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumMacroTokens", ExactSpelling = true)]
    public static extern int Cursor_getNumMacroTokens(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasCatchAll", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasCatchAll(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getHasObjCAvailabilityVersion", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getHasObjCAvailabilityVersion(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsDelegateInitCall", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsDelegateInitCall(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsMessagingGetter", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsMessagingGetter(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getIsMessagingSetter", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getIsMessagingSetter(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumCatchStmts", ExactSpelling = true)]
    public static extern int Cursor_getNumCatchStmts(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getNumObjCLiteralElements", ExactSpelling = true)]
    public static extern int Cursor_getNumObjCLiteralElements(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjCAvailabilityVersion", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getObjCAvailabilityVersion(CXCursor C, [NativeTypeName("llvm::VersionTuple *")] VersionTuple* version);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjCDictionaryKey", ExactSpelling = true)]
    public static extern CXCursor Cursor_getObjCDictionaryKey(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjCDictionaryValue", ExactSpelling = true)]
    public static extern CXCursor Cursor_getObjCDictionaryValue(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjCMessageReceiverKind", ExactSpelling = true)]
    public static extern CX_ObjCMessageReceiverKind Cursor_getObjCMessageReceiverKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjCMessageReceiverType", ExactSpelling = true)]
    public static extern CXType Cursor_getObjCMessageReceiverType(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjCPropertyRefClassReceiver", ExactSpelling = true)]
    public static extern CXCursor Cursor_getObjCPropertyRefClassReceiver(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getObjCPropertyRefReceiverKind", ExactSpelling = true)]
    public static extern CX_ObjCPropertyRefReceiverKind Cursor_getObjCPropertyRefReceiverKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getSetAtIndexMethodDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getSetAtIndexMethodDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTypeOperand", ExactSpelling = true)]
    public static extern CXType Cursor_getTypeOperand(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTypeParam", ExactSpelling = true)]
    public static extern CXCursor Cursor_getTypeParam(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnaryExprOrTypeTraitKind", ExactSpelling = true)]
    public static extern CX_UnaryExprOrTypeTrait Cursor_getUnaryExprOrTypeTraitKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUnderlyingDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getUnderlyingDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUninstantiatedDefaultArg", ExactSpelling = true)]
    public static extern CXCursor Cursor_getUninstantiatedDefaultArg(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUsedContext", ExactSpelling = true)]
    public static extern CXCursor Cursor_getUsedContext(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getUsingEnumDeclEnumDecl", ExactSpelling = true)]
    public static extern CXCursor Cursor_getUsingEnumDeclEnumDecl(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTypeParamHasExplicitBound", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getTypeParamHasExplicitBound(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTypeParamVariance", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Cursor_getTypeParamVariance(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getTypeTrait", ExactSpelling = true)]
    public static extern CX_TypeTrait Cursor_getTypeTrait(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getValueKind", ExactSpelling = true)]
    public static extern CX_ExprValueKind Cursor_getValueKind(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getVBase", ExactSpelling = true)]
    public static extern CXCursor Cursor_getVBase(CXCursor C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getDtorVtblIdx", ExactSpelling = true)]
    [return: NativeTypeName("int64_t")]
    public static extern long Cursor_getDtorVtblIdx(CXCursor C, CX_DestructorType dtor);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_getVtblIdx", ExactSpelling = true)]
    [return: NativeTypeName("int64_t")]
    public static extern long Cursor_getVtblIdx(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Cursor_prettyPrintAttribute", ExactSpelling = true)]
    public static extern CXString Cursor_prettyPrintAttribute(CXCursor C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_getVersion", ExactSpelling = true)]
    public static extern CXString getVersion();

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_dispose", ExactSpelling = true)]
    public static extern void TemplateArgument_dispose(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getAsDecl", ExactSpelling = true)]
    public static extern CXCursor TemplateArgument_getAsDecl(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getAsExpr", ExactSpelling = true)]
    public static extern CXCursor TemplateArgument_getAsExpr(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getAsIntegral", ExactSpelling = true)]
    [return: NativeTypeName("int64_t")]
    public static extern long TemplateArgument_getAsIntegral(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getAsTemplate", ExactSpelling = true)]
    public static extern CX_TemplateName TemplateArgument_getAsTemplate(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getAsTemplateOrTemplatePattern", ExactSpelling = true)]
    public static extern CX_TemplateName TemplateArgument_getAsTemplateOrTemplatePattern(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getAsType", ExactSpelling = true)]
    public static extern CXType TemplateArgument_getAsType(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getDependence", ExactSpelling = true)]
    public static extern CX_TemplateArgumentDependence TemplateArgument_getDependence(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getIntegralType", ExactSpelling = true)]
    public static extern CXType TemplateArgument_getIntegralType(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getIsDefaulted", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint TemplateArgument_getIsDefaulted(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getNonTypeTemplateArgumentType", ExactSpelling = true)]
    public static extern CXType TemplateArgument_getNonTypeTemplateArgumentType(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getNullPtrType", ExactSpelling = true)]
    public static extern CXType TemplateArgument_getNullPtrType(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getNumPackElements", ExactSpelling = true)]
    public static extern int TemplateArgument_getNumPackElements(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getNumTemplateExpansions", ExactSpelling = true)]
    public static extern int TemplateArgument_getNumTemplateExpansions(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getPackElement", ExactSpelling = true)]
    public static extern CX_TemplateArgument TemplateArgument_getPackElement(CX_TemplateArgument T, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getPackExpansionPattern", ExactSpelling = true)]
    public static extern CX_TemplateArgument TemplateArgument_getPackExpansionPattern(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgument_getParamTypeForDecl", ExactSpelling = true)]
    public static extern CXType TemplateArgument_getParamTypeForDecl(CX_TemplateArgument T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getArgument", ExactSpelling = true)]
    public static extern CX_TemplateArgument TemplateArgumentLoc_getArgument(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getLocation", ExactSpelling = true)]
    public static extern CXSourceLocation TemplateArgumentLoc_getLocation(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getSourceDeclExpression", ExactSpelling = true)]
    public static extern CXCursor TemplateArgumentLoc_getSourceDeclExpression(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getSourceExpression", ExactSpelling = true)]
    public static extern CXCursor TemplateArgumentLoc_getSourceExpression(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getSourceIntegralExpression", ExactSpelling = true)]
    public static extern CXCursor TemplateArgumentLoc_getSourceIntegralExpression(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getSourceNullPtrExpression", ExactSpelling = true)]
    public static extern CXCursor TemplateArgumentLoc_getSourceNullPtrExpression(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getSourceRange", ExactSpelling = true)]
    public static extern CXSourceRange TemplateArgumentLoc_getSourceRange(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getSourceRangeRaw", ExactSpelling = true)]
    public static extern CXSourceRange TemplateArgumentLoc_getSourceRangeRaw(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getTemplateEllipsisLoc", ExactSpelling = true)]
    public static extern CXSourceLocation TemplateArgumentLoc_getTemplateEllipsisLoc(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateArgumentLoc_getTemplateNameLoc", ExactSpelling = true)]
    public static extern CXSourceLocation TemplateArgumentLoc_getTemplateNameLoc(CX_TemplateArgumentLoc T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateName_getAsTemplateDecl", ExactSpelling = true)]
    public static extern CXCursor TemplateName_getAsTemplateDecl(CX_TemplateName T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateName_getContainsUnexpandedParameterPack", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint TemplateName_getContainsUnexpandedParameterPack(CX_TemplateName T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateName_getIsDependent", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint TemplateName_getIsDependent(CX_TemplateName T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateName_getIsInstantiationDependent", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint TemplateName_getIsInstantiationDependent(CX_TemplateName T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_TemplateName_getUnderlying", ExactSpelling = true)]
    public static extern CX_TemplateName TemplateName_getUnderlying(CX_TemplateName T);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_desugar", ExactSpelling = true)]
    public static extern CXType Type_desugar(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getAddrSpaceExpr", ExactSpelling = true)]
    public static extern CXCursor Type_getAddrSpaceExpr(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getAdjustedType", ExactSpelling = true)]
    public static extern CXType Type_getAdjustedType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getAttrKind", ExactSpelling = true)]
    public static extern CX_AttrKind Type_getAttrKind(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getAutoTypeKeyword", ExactSpelling = true)]
    public static extern CX_AutoTypeKeyword Type_getAutoTypeKeyword(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getBaseType", ExactSpelling = true)]
    public static extern CXType Type_getBaseType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getColumnExpr", ExactSpelling = true)]
    public static extern CXCursor Type_getColumnExpr(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getContainsUnexpandedParameterPack", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getContainsUnexpandedParameterPack(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDecayedType", ExactSpelling = true)]
    public static extern CXType Type_getDecayedType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDeclaration", ExactSpelling = true)]
    public static extern CXCursor Type_getDeclaration(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDeducedType", ExactSpelling = true)]
    public static extern CXType Type_getDeducedType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDependence", ExactSpelling = true)]
    public static extern CX_TypeDependence Type_getDependence(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getDepth", ExactSpelling = true)]
    public static extern int Type_getDepth(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getElementType", ExactSpelling = true)]
    public static extern CXType Type_getElementType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getEquivalentType", ExactSpelling = true)]
    public static extern CXType Type_getEquivalentType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getExceptionSpecType", ExactSpelling = true)]
    public static extern CX_ExceptionSpecificationType Type_getExceptionSpecType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getExceptionType", ExactSpelling = true)]
    public static extern CXType Type_getExceptionType(CXType C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getHasFloatingRepresentation", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getHasFloatingRepresentation(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getHasIntegerRepresentation", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getHasIntegerRepresentation(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getHasPointerRepresentation", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getHasPointerRepresentation(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getHasTrailingReturn", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getHasTrailingReturn(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIndex", ExactSpelling = true)]
    public static extern int Type_getIndex(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getInjectedSpecializationType", ExactSpelling = true)]
    public static extern CXType Type_getInjectedSpecializationType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getInjectedTST", ExactSpelling = true)]
    public static extern CXType Type_getInjectedTST(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsAggregateType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsAggregateType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsArithmeticType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsArithmeticType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsConst", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsConst(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsConstrained", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsConstrained(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsDecltypeAuto", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsDecltypeAuto(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsDependentType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsDependentType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsFloatingType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsFloatingType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsGNUAutoType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsGNUAutoType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsInstantiationDependentType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsInstantiationDependentType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCInstanceType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCInstanceType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjectType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjectType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsParameterPack", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsParameterPack(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsRealFloatingType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsRealFloatingType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsScalarType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsScalarType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsSigned", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsSigned(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsSugared", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsSugared(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsTypeAlias", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsTypeAlias(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsUnsigned", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsUnsigned(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsVariablyModifiedType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsVariablyModifiedType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getKeyword", ExactSpelling = true)]
    public static extern CX_ElaboratedTypeKeyword Type_getKeyword(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getModifiedType", ExactSpelling = true)]
    public static extern CXType Type_getModifiedType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getNoexceptExpr", ExactSpelling = true)]
    public static extern CXCursor Type_getNoexceptExpr(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getNumBits", ExactSpelling = true)]
    public static extern int Type_getNumBits(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getNumBitsExpr", ExactSpelling = true)]
    public static extern CXCursor Type_getNumBitsExpr(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getNumColumns", ExactSpelling = true)]
    public static extern int Type_getNumColumns(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getNumElementsFlattened", ExactSpelling = true)]
    public static extern int Type_getNumElementsFlattened(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getNumRows", ExactSpelling = true)]
    public static extern int Type_getNumRows(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getOriginalType", ExactSpelling = true)]
    public static extern CXType Type_getOriginalType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getSubstTemplateTypeParamAssociatedDecl", ExactSpelling = true)]
    public static extern CXCursor Type_getSubstTemplateTypeParamAssociatedDecl(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getOwnedTagDecl", ExactSpelling = true)]
    public static extern CXCursor Type_getOwnedTagDecl(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getPointeeType", ExactSpelling = true)]
    public static extern CXType Type_getPointeeType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getRowExpr", ExactSpelling = true)]
    public static extern CXCursor Type_getRowExpr(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getSizeExpr", ExactSpelling = true)]
    public static extern CXCursor Type_getSizeExpr(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateArgument", ExactSpelling = true)]
    public static extern CX_TemplateArgument Type_getTemplateArgument(CXType C, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTemplateName", ExactSpelling = true)]
    public static extern CX_TemplateName Type_getTemplateName(CXType C);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getTypeClass", ExactSpelling = true)]
    public static extern CX_TypeClass Type_getTypeClass(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getUnderlyingExpr", ExactSpelling = true)]
    public static extern CXCursor Type_getUnderlyingExpr(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCClassType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCClassType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCIdType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCIdType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCKindOfType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCKindOfType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCKindOfTypeAsWritten", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCKindOfTypeAsWritten(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCObjectSpecialized", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCObjectSpecialized(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCObjectSpecializedAsWritten", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCObjectSpecializedAsWritten(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCQualifiedClassType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCQualifiedClassType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getIsObjCQualifiedIdType", ExactSpelling = true)]
    [return: NativeTypeName("unsigned int")]
    public static extern uint Type_getIsObjCQualifiedIdType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getNumObjCTypeParamProtocols", ExactSpelling = true)]
    public static extern int Type_getNumObjCTypeParamProtocols(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getObjCSuperClassType", ExactSpelling = true)]
    public static extern CXType Type_getObjCSuperClassType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getObjCTypeParamProtocol", ExactSpelling = true)]
    public static extern CXCursor Type_getObjCTypeParamProtocol(CXType CT, [NativeTypeName("unsigned int")] uint i);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getUnderlyingType", ExactSpelling = true)]
    public static extern CXType Type_getUnderlyingType(CXType CT);

    [DllImport("libClangSharp", CallingConvention = CallingConvention.Cdecl, EntryPoint = "clangsharp_Type_getVectorKind", ExactSpelling = true)]
    public static extern CX_VectorKind Type_getVectorKind(CXType CT);
}
