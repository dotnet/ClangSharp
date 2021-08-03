// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_BinaryOperatorKind
    {
        CX_BO_Invalid,
        CX_BO_PtrMemD,
        CX_BO_PtrMemI,
        CX_BO_Mul,
        CX_BO_Div,
        CX_BO_Rem,
        CX_BO_Add,
        CX_BO_Sub,
        CX_BO_Shl,
        CX_BO_Shr,
        CX_BO_Cmp,
        CX_BO_LT,
        CX_BO_GT,
        CX_BO_LE,
        CX_BO_GE,
        CX_BO_EQ,
        CX_BO_NE,
        CX_BO_And,
        CX_BO_Xor,
        CX_BO_Or,
        CX_BO_LAnd,
        CX_BO_LOr,
        CX_BO_Assign,
        CX_BO_MulAssign,
        CX_BO_DivAssign,
        CX_BO_RemAssign,
        CX_BO_AddAssign,
        CX_BO_SubAssign,
        CX_BO_ShlAssign,
        CX_BO_ShrAssign,
        CX_BO_AndAssign,
        CX_BO_XorAssign,
        CX_BO_OrAssign,
        CX_BO_Comma,
    }
}
