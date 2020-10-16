// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-11.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXTemplateArgumentKind
    {
        CXTemplateArgumentKind_Null,
        CXTemplateArgumentKind_Type,
        CXTemplateArgumentKind_Declaration,
        CXTemplateArgumentKind_NullPtr,
        CXTemplateArgumentKind_Integral,
        CXTemplateArgumentKind_Template,
        CXTemplateArgumentKind_TemplateExpansion,
        CXTemplateArgumentKind_Expression,
        CXTemplateArgumentKind_Pack,
        CXTemplateArgumentKind_Invalid,
    }
}
