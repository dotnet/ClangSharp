// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_CapturedRegionKind
    {
        CX_CR_Invalid,
        CX_CR_Default,
        CX_CR_ObjCAtFinally,
        CX_CR_OpenMP,
    }
}
