// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_UnaryExprOrTypeTrait
    {
        CX_UETT_Invalid,
        CX_UETT_SizeOf,
        CX_UETT_AlignOf,
        CX_UETT_PreferredAlignOf,
        CX_UETT_VecStep,
        CX_UETT_OpenMPRequiredSimdAlign,
        CX_UETT_Last = -1 + 1 + 1 + 1 + 1 + 1,
    }
}
