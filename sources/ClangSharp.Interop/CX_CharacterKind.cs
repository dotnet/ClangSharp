// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_CharacterKind
    {
        CX_CLK_Invalid,
        CX_CLK_Ascii,
        CX_CLK_Wide,
        CX_CLK_UTF8,
        CX_CLK_UTF16,
        CX_CLK_UTF32,
    }
}
