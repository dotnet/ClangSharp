// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_InitializationStyle
{
    CX_IS_Invalid,
    CX_IS_CInit = 1,
    CX_IS_CallInit = 2,
    CX_IS_ListInit = 3,
    CX_IS_ParenListInit = 4,
}
