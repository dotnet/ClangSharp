// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_InclusionDirectiveKind
{
    CX_IDK_Invalid,
    CX_IDK_Include = 1,
    CX_IDK_Import = 2,
    CX_IDK_IncludeNext = 3,
    CX_IDK_IncludeMacros = 4,
}
