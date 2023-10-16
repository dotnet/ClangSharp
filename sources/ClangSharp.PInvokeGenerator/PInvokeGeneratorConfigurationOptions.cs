// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp;

[Flags]
public enum PInvokeGeneratorConfigurationOptions : long
{
    None = 0,

    GenerateMultipleFiles = 1L << 0,

    GenerateUnixTypes = 1L << 1,

    NoDefaultRemappings = 1L << 2,

    GenerateCompatibleCode = 1L << 3,

    ExcludeNIntCodegen = 1L << 4,

    ExcludeFnptrCodegen = 1L << 5,

    LogExclusions = 1L << 6,

    LogVisitedFiles = 1L << 7,

    GenerateExplicitVtbls = 1L << 8,

    GenerateTestsNUnit = 1L << 9,

    GenerateTestsXUnit = 1L << 10,

    GenerateMacroBindings = 1L << 11,

    ExcludeComProxies = 1L << 12,

    ExcludeEmptyRecords = 1L << 13,

    ExcludeEnumOperators = 1L << 14,

    GenerateAggressiveInlining = 1L << 15,

    ExcludeFunctionsWithBody = 1L << 16,

    ExcludeAnonymousFieldHelpers = 1L << 17,

    LogPotentialTypedefRemappings = 1L << 18,

    GenerateCppAttributes = 1L << 19,

    GenerateNativeInheritanceAttribute = 1L << 20,

    DontUseUsingStaticsForEnums = 1L << 21,

    GenerateVtblIndexAttribute = 1L << 22,

    GeneratePreviewCode = 1L << 23,

    GenerateTemplateBindings = 1L << 24,

    GenerateSourceLocationAttribute = 1L << 25,

    GenerateUnmanagedConstants = 1L << 26,

    GenerateHelperTypes = 1L << 27,

    GenerateTrimmableVtbls = 1L << 28,

    GenerateMarkerInterfaces = 1L << 29,

    GenerateFileScopedNamespaces = 1L << 30,

    GenerateSetsLastSystemErrorAttribute = 1L << 31,

    GenerateDocIncludes = 1L << 32,

    GenerateGuidMember = 1L << 33,

    GenerateLatestCode = 1L << 34,

    GenerateNativeBitfieldAttribute = 1L << 35,

    GenerateDisableRuntimeMarshalling = 1L << 36,

    GenerateCallConvMemberFunction = 1L << 37,
}
