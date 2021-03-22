// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop
{
    public enum CX_ConstructionKind
    {
        CX_CK_Invalid,
        CX_CK_Complete,
        CX_CK_NonVirtualBase,
        CX_CK_VirtualBase,
        CX_CK_Delegating
    }
}
