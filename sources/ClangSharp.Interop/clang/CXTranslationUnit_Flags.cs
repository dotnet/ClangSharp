// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXTranslationUnit_Flags
{
    CXTranslationUnit_None = 0x0,
    CXTranslationUnit_DetailedPreprocessingRecord = 0x01,
    CXTranslationUnit_Incomplete = 0x02,
    CXTranslationUnit_PrecompiledPreamble = 0x04,
    CXTranslationUnit_CacheCompletionResults = 0x08,
    CXTranslationUnit_ForSerialization = 0x10,
    CXTranslationUnit_CXXChainedPCH = 0x20,
    CXTranslationUnit_SkipFunctionBodies = 0x40,
    CXTranslationUnit_IncludeBriefCommentsInCodeCompletion = 0x80,
    CXTranslationUnit_CreatePreambleOnFirstParse = 0x100,
    CXTranslationUnit_KeepGoing = 0x200,
    CXTranslationUnit_SingleFileParse = 0x400,
    CXTranslationUnit_LimitSkipFunctionBodiesToPreamble = 0x800,
    CXTranslationUnit_IncludeAttributedTypes = 0x1000,
    CXTranslationUnit_VisitImplicitAttributes = 0x2000,
    CXTranslationUnit_IgnoreNonErrorsFromIncludedFiles = 0x4000,
    CXTranslationUnit_RetainExcludedConditionalBlocks = 0x8000,
}
