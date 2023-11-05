// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-17.0.4/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXDiagnosticDisplayOptions
{
    CXDiagnostic_DisplaySourceLocation = 0x01,
    CXDiagnostic_DisplayColumn = 0x02,
    CXDiagnostic_DisplaySourceRanges = 0x04,
    CXDiagnostic_DisplayOption = 0x08,
    CXDiagnostic_DisplayCategoryId = 0x10,
    CXDiagnostic_DisplayCategoryName = 0x20,
}
