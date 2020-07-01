// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/master/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_FloatingSemantics
    {
        CX_FLK_Invalid,
        CX_FLK_IEEEhalf,
        CX_FLK_IEEEsingle,
        CX_FLK_IEEEdouble,
        CX_FLK_x87DoubleExtended,
        CX_FLK_IEEEquad,
        CX_FLK_PPCDoubleDouble,
    }
}
