// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-20.1.2/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXIdxEntityCXXTemplateKind
{
    CXIdxEntity_NonTemplate = 0,
    CXIdxEntity_Template = 1,
    CXIdxEntity_TemplatePartialSpecialization = 2,
    CXIdxEntity_TemplateSpecialization = 3,
}
