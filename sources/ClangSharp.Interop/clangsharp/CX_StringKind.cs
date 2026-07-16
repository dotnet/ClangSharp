// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_StringKind
{
    CX_SLK_Invalid,
    CX_SLK_Ordinary = 1,
    CX_SLK_Wide = 2,
    CX_SLK_UTF8 = 3,
    CX_SLK_UTF16 = 4,
    CX_SLK_UTF32 = 5,
    CX_SLK_Unevaluated = 6,
}
