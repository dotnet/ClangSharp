// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_UnaryOperatorKind
{
    CX_UO_Invalid,
    CX_UO_PostInc,
    CX_UO_PostDec,
    CX_UO_PreInc,
    CX_UO_PreDec,
    CX_UO_AddrOf,
    CX_UO_Deref,
    CX_UO_Plus,
    CX_UO_Minus,
    CX_UO_Not,
    CX_UO_LNot,
    CX_UO_Real,
    CX_UO_Imag,
    CX_UO_Extension,
    CX_UO_Coawait,
}
