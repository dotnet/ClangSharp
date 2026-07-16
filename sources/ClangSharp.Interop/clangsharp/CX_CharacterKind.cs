// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_CharacterKind
{
    CX_CLK_Invalid,
    CX_CLK_Ascii = 1,
    CX_CLK_Wide = 2,
    CX_CLK_UTF8 = 3,
    CX_CLK_UTF16 = 4,
    CX_CLK_UTF32 = 5,
}
