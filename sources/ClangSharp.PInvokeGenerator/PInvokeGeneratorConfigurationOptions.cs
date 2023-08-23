// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp;

[Flags]
public enum PInvokeGeneratorConfigurationOptions : ulong
{
    None = 0,

    GenerateMultipleFiles = 1UL << 0,

    GenerateUnixTypes = 1UL << 1,

    NoDefaultRemappings = 1UL << 2,

    GenerateCompatibleCode = 1UL << 3,

    ExcludeNIntCodegen = 1UL << 4,

    ExcludeFnptrCodegen = 1UL << 5,

    LogExclusions = 1UL << 6,

    LogVisitedFiles = 1UL << 7,

    GenerateExplicitVtbls = 1UL << 8,

    GenerateTestsNUnit = 1UL << 9,

    GenerateTestsXUnit = 1UL << 10,

    GenerateMacroBindings = 1UL << 11,

    ExcludeComProxies = 1UL << 12,

    ExcludeEmptyRecords = 1UL << 13,

    ExcludeEnumOperators = 1UL << 14,

    GenerateAggressiveInlining = 1UL << 15,

    ExcludeFunctionsWithBody = 1UL << 16,

    ExcludeAnonymousFieldHelpers = 1UL << 17,

    LogPotentialTypedefRemappings = 1UL << 18,

    GenerateCppAttributes = 1UL << 19,

    GenerateNativeInheritanceAttribute = 1UL << 20,

    DontUseUsingStaticsForEnums = 1UL << 21,

    GenerateVtblIndexAttribute = 1UL << 22,

    GeneratePreviewCode = 1UL << 23,

    GenerateTemplateBindings = 1UL << 24,

    GenerateSourceLocationAttribute = 1UL << 25,

    GenerateUnmanagedConstants = 1UL << 26,

    GenerateHelperTypes = 1UL << 27,

    GenerateTrimmableVtbls = 1UL << 28,

    GenerateMarkerInterfaces = 1UL << 29,

    GenerateFileScopedNamespaces = 1UL << 30,

    GenerateSetsLastSystemErrorAttribute = 1UL << 31,

    GenerateDocIncludes = 1UL << 32,

    GenerateGuidMember = 1UL << 33,

    GenerateLatestCode = 1UL << 34,

    GenerateNativeBitfieldAttribute = 1UL << 35,

    GenerateDisableRuntimeMarshalling = 1UL << 36,
}
