// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-14.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXIndexOptFlags
    {
        CXIndexOpt_None = 0x0,
        CXIndexOpt_SuppressRedundantRefs = 0x1,
        CXIndexOpt_IndexFunctionLocalSymbols = 0x2,
        CXIndexOpt_IndexImplicitTemplateInstantiations = 0x4,
        CXIndexOpt_SuppressWarnings = 0x8,
        CXIndexOpt_SkipParsedBodiesInSession = 0x10,
    }
}
