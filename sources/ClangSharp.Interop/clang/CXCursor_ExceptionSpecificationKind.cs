// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-21.1.8/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXCursor_ExceptionSpecificationKind
{
    CXCursor_ExceptionSpecificationKind_None,
    CXCursor_ExceptionSpecificationKind_DynamicNone,
    CXCursor_ExceptionSpecificationKind_Dynamic,
    CXCursor_ExceptionSpecificationKind_MSAny,
    CXCursor_ExceptionSpecificationKind_BasicNoexcept,
    CXCursor_ExceptionSpecificationKind_ComputedNoexcept,
    CXCursor_ExceptionSpecificationKind_Unevaluated,
    CXCursor_ExceptionSpecificationKind_Uninstantiated,
    CXCursor_ExceptionSpecificationKind_Unparsed,
    CXCursor_ExceptionSpecificationKind_NoThrow,
}
