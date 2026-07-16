// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_VectorKind
{
    CX_VECK_Invalid,
    CX_VECK_Generic = 1,
    CX_VECK_AltiVecVector = 2,
    CX_VECK_AltiVecPixel = 3,
    CX_VECK_AltiVecBool = 4,
    CX_VECK_Neon = 5,
    CX_VECK_NeonPoly = 6,
    CX_VECK_SveFixedLengthData = 7,
    CX_VECK_SveFixedLengthPredicate = 8,
    CX_VECK_RVVFixedLengthData = 9,
    CX_VECK_RVVFixedLengthMask = 10,
    CX_VECK_RVVFixedLengthMask_1 = 11,
    CX_VECK_RVVFixedLengthMask_2 = 12,
    CX_VECK_RVVFixedLengthMask_4 = 13,
}
