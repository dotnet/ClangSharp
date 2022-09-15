// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-14.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXPrintingPolicyProperty
{
    CXPrintingPolicy_Indentation,
    CXPrintingPolicy_SuppressSpecifiers,
    CXPrintingPolicy_SuppressTagKeyword,
    CXPrintingPolicy_IncludeTagDefinition,
    CXPrintingPolicy_SuppressScope,
    CXPrintingPolicy_SuppressUnwrittenScope,
    CXPrintingPolicy_SuppressInitializers,
    CXPrintingPolicy_ConstantArraySizeAsWritten,
    CXPrintingPolicy_AnonymousTagLocations,
    CXPrintingPolicy_SuppressStrongLifetime,
    CXPrintingPolicy_SuppressLifetimeQualifiers,
    CXPrintingPolicy_SuppressTemplateArgsInCXXConstructors,
    CXPrintingPolicy_Bool,
    CXPrintingPolicy_Restrict,
    CXPrintingPolicy_Alignof,
    CXPrintingPolicy_UnderscoreAlignof,
    CXPrintingPolicy_UseVoidForZeroParams,
    CXPrintingPolicy_TerseOutput,
    CXPrintingPolicy_PolishForDeclaration,
    CXPrintingPolicy_Half,
    CXPrintingPolicy_MSWChar,
    CXPrintingPolicy_IncludeNewlines,
    CXPrintingPolicy_MSVCFormatting,
    CXPrintingPolicy_ConstantsAsWritten,
    CXPrintingPolicy_SuppressImplicitBase,
    CXPrintingPolicy_FullyQualifiedName,
    CXPrintingPolicy_LastProperty = CXPrintingPolicy_FullyQualifiedName,
}
