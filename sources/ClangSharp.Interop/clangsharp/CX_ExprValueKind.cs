// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_ExprValueKind
{
    CX_VK_Invalid,
    CX_VK_PRValue = 1,
    CX_VK_LValue = 2,
    CX_VK_XValue = 3,
}
