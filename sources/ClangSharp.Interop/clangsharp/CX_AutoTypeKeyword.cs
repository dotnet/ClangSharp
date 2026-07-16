// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_AutoTypeKeyword
{
    CX_ATK_Invalid,
    CX_ATK_Auto = 1,
    CX_ATK_DecltypeAuto = 2,
    CX_ATK_GNUAutoType = 3,
}
