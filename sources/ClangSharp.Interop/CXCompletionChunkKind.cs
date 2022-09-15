// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-14.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop;

public enum CXCompletionChunkKind
{
    CXCompletionChunk_Optional,
    CXCompletionChunk_TypedText,
    CXCompletionChunk_Text,
    CXCompletionChunk_Placeholder,
    CXCompletionChunk_Informative,
    CXCompletionChunk_CurrentParameter,
    CXCompletionChunk_LeftParen,
    CXCompletionChunk_RightParen,
    CXCompletionChunk_LeftBracket,
    CXCompletionChunk_RightBracket,
    CXCompletionChunk_LeftBrace,
    CXCompletionChunk_RightBrace,
    CXCompletionChunk_LeftAngle,
    CXCompletionChunk_RightAngle,
    CXCompletionChunk_Comma,
    CXCompletionChunk_ResultType,
    CXCompletionChunk_Colon,
    CXCompletionChunk_SemiColon,
    CXCompletionChunk_Equal,
    CXCompletionChunk_HorizontalSpace,
    CXCompletionChunk_VerticalSpace,
}
