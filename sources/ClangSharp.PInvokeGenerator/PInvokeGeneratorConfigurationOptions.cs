// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp
{
    [Flags]
    public enum PInvokeGeneratorConfigurationOptions
    {
        None = 0,

        GenerateMultipleFiles = 1 << 0,

        GenerateUnixTypes = 1 << 1,

        NoDefaultRemappings = 1 << 2,

        GenerateCompatibleCode = 1 << 3,

        ExcludeNIntCodegen = 1 << 4,

        ExcludeFnptrCodegen = 1 << 5,

        LogExclusions = 1 << 6,

        LogVisitedFiles = 1 << 7,

        GenerateExplicitVtbls = 1 << 8,

        GenerateTestsNUnit = 1 << 9,

        GenerateTestsXUnit = 1 << 10,

        GenerateMacroBindings = 1 << 11,

        ExcludeComProxies = 1 << 12,

        ExcludeEmptyRecords = 1 << 13,

        ExcludeEnumOperators = 1 << 14,

        GenerateAggressiveInlining = 1 << 15,

        ExcludeFunctionsWithBody = 1 << 16,

        ExcludeAnonymousFieldHelpers = 1 << 17,

        LogPotentialTypedefRemappings = 1 << 18,

        GenerateCppAttributes = 1 << 19,

        GenerateNativeInheritanceAttribute = 1 << 20,

        DontUseUsingStaticsForEnums = 1 << 21,

        GenerateVtblIndexAttribute = 1 << 22,

        GeneratePreviewCode = 1 << 23,

        GenerateTemplateBindings = 1 << 24,

        GenerateSourceLocationAttribute = 1 << 25,

        GenerateUnmanagedConstants = 1 << 26,

        GenerateHelperTypes = 1 << 27,
    }
}
