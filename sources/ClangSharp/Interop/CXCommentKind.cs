// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-10.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
    public enum CXCommentKind
    {
        CXComment_Null = 0,
        CXComment_Text = 1,
        CXComment_InlineCommand = 2,
        CXComment_HTMLStartTag = 3,
        CXComment_HTMLEndTag = 4,
        CXComment_Paragraph = 5,
        CXComment_BlockCommand = 6,
        CXComment_ParamCommand = 7,
        CXComment_TParamCommand = 8,
        CXComment_VerbatimBlockCommand = 9,
        CXComment_VerbatimBlockLine = 10,
        CXComment_VerbatimLine = 11,
        CXComment_FullComment = 12,
    }
}
