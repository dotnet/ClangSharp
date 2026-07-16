// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_IfStatementKind
{
    CX_ISK_Invalid,
    CX_ISK_Ordinary = 1,
    CX_ISK_Constexpr = 2,
    CX_ISK_ConstevalNonNegated = 3,
    CX_ISK_ConstevalNegated = 4,
}
