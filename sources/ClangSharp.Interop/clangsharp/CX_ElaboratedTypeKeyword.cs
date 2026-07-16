// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_ElaboratedTypeKeyword
{
    CX_ETK_Invalid,
    CX_ETK_Struct = 1,
    CX_ETK_Interface = 2,
    CX_ETK_Union = 3,
    CX_ETK_Class = 4,
    CX_ETK_Enum = 5,
    CX_ETK_Typename = 6,
    CX_ETK_None = 7,
}
