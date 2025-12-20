// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_UnaryExprOrTypeTrait
{
    CX_UETT_Invalid,
    CX_UETT_SizeOf,
    CX_UETT_DataSizeOf,
    CX_UETT_CountOf,
    CX_UETT_AlignOf,
    CX_UETT_PreferredAlignOf,
    CX_UETT_PtrAuthTypeDiscriminator,
    CX_UETT_VecStep,
    CX_UETT_OpenMPRequiredSimdAlign,
    CX_UETT_VectorElements,
    CX_UETT_Last = -1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1 + 1,
}
