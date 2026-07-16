// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_FloatingSemantics
{
    CX_FLK_Invalid,
    CX_FLK_IEEEhalf = 1,
    CX_FLK_BFloat = 2,
    CX_FLK_IEEEsingle = 3,
    CX_FLK_IEEEdouble = 4,
    CX_FLK_IEEEquad = 5,
    CX_FLK_PPCDoubleDouble = 6,
    CX_FLK_PPCDoubleDoubleLegacy = 7,
    CX_FLK_Float8E5M2 = 8,
    CX_FLK_Float8E5M2FNUZ = 9,
    CX_FLK_Float8E4M3 = 10,
    CX_FLK_Float8E4M3FN = 11,
    CX_FLK_Float8E4M3FNUZ = 12,
    CX_FLK_Float8E4M3B11FNUZ = 13,
    CX_FLK_Float8E3M4 = 14,
    CX_FLK_FloatTF32 = 15,
    CX_FLK_Float8E8M0FNU = 16,
    CX_FLK_Float6E3M2FN = 17,
    CX_FLK_Float6E2M3FN = 18,
    CX_FLK_Float4E2M1FN = 19,
    CX_FLK_x87DoubleExtended = 20,
    CX_FLK_MaxSemantics = CX_FLK_x87DoubleExtended,
}
