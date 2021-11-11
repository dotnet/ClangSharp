// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

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
