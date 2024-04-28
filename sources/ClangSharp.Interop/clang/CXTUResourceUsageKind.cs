// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-18.1.3/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXTUResourceUsageKind
{
    CXTUResourceUsage_AST = 1,
    CXTUResourceUsage_Identifiers = 2,
    CXTUResourceUsage_Selectors = 3,
    CXTUResourceUsage_GlobalCompletionResults = 4,
    CXTUResourceUsage_SourceManagerContentCache = 5,
    CXTUResourceUsage_AST_SideTables = 6,
    CXTUResourceUsage_SourceManager_Membuffer_Malloc = 7,
    CXTUResourceUsage_SourceManager_Membuffer_MMap = 8,
    CXTUResourceUsage_ExternalASTSource_Membuffer_Malloc = 9,
    CXTUResourceUsage_ExternalASTSource_Membuffer_MMap = 10,
    CXTUResourceUsage_Preprocessor = 11,
    CXTUResourceUsage_PreprocessingRecord = 12,
    CXTUResourceUsage_SourceManager_DataStructures = 13,
    CXTUResourceUsage_Preprocessor_HeaderSearch = 14,
    CXTUResourceUsage_MEMORY_IN_BYTES_BEGIN = CXTUResourceUsage_AST,
    CXTUResourceUsage_MEMORY_IN_BYTES_END = CXTUResourceUsage_Preprocessor_HeaderSearch,
    CXTUResourceUsage_First = CXTUResourceUsage_AST,
    CXTUResourceUsage_Last = CXTUResourceUsage_Preprocessor_HeaderSearch,
}
