// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_LiteralOperatorKind
{
    CX_LOK_Invalid,
    CX_LOK_Raw = 1,
    CX_LOK_Template = 2,
    CX_LOK_Integer = 3,
    CX_LOK_Floating = 4,
    CX_LOK_String = 5,
    CX_LOK_Character = 6,
}
