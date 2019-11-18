// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

// Ported from https://github.com/llvm/llvm-project/tree/llvmorg-9.0.0/clang/include/clang-c
// Original source is Copyright (c) the LLVM Project and Contributors. Licensed under the Apache License v2.0 with LLVM Exceptions. See NOTICE.txt in the project root for license information.

namespace ClangSharp.Interop
{
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
}
