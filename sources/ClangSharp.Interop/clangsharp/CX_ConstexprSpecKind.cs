// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_ConstexprSpecKind
{
    CX_CSK_Invalid,
    CX_CSK_Unspecified = 1,
    CX_CSK_Constexpr = 2,
    CX_CSK_Consteval = 3,
    CX_CSK_Constinit = 4,
}
