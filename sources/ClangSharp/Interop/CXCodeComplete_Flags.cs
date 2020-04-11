// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-10.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXCodeComplete_Flags
    {
        CXCodeComplete_IncludeMacros = 0x01,
        CXCodeComplete_IncludeCodePatterns = 0x02,
        CXCodeComplete_IncludeBriefComments = 0x04,
        CXCodeComplete_SkipPreamble = 0x08,
        CXCodeComplete_IncludeCompletionsWithFixIts = 0x10,
    }
}
