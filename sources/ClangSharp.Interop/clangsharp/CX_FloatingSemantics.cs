// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_FloatingSemantics
{
    CX_FLK_Invalid,
    CX_FLK_IEEEhalf,
    CX_FLK_BFloat,
    CX_FLK_IEEEsingle,
    CX_FLK_IEEEdouble,
    CX_FLK_IEEEquad,
    CX_FLK_PPCDoubleDouble,
    CX_FLK_PPCDoubleDoubleLegacy,
    CX_FLK_Float8E5M2,
    CX_FLK_Float8E5M2FNUZ,
    CX_FLK_Float8E4M3,
    CX_FLK_Float8E4M3FN,
    CX_FLK_Float8E4M3FNUZ,
    CX_FLK_Float8E4M3B11FNUZ,
    CX_FLK_Float8E3M4,
    CX_FLK_FloatTF32,
    CX_FLK_Float8E8M0FNU,
    CX_FLK_Float6E3M2FN,
    CX_FLK_Float6E2M3FN,
    CX_FLK_Float4E2M1FN,
    CX_FLK_x87DoubleExtended,
    CX_FLK_MaxSemantics = CX_FLK_x87DoubleExtended,
}
