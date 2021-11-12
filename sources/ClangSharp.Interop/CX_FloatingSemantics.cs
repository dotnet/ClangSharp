// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_FloatingSemantics
    {
        CX_FLK_Invalid,
        CX_FLK_IEEEhalf,
        CX_FLK_BFloat,
        CX_FLK_IEEEsingle,
        CX_FLK_IEEEdouble,
        CX_FLK_x87DoubleExtended,
        CX_FLK_IEEEquad,
        CX_FLK_PPCDoubleDouble,
    }
}
