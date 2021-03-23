// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

using System;

namespace ClangSharp.Interop
{
    [Flags]
    public enum CX_ExprDependence
    {
        CX_ED_None = 0,
        CX_ED_UnexpandedPack = 1,
        CX_ED_Instantiation = 2,
        CX_ED_Type = 4,
        CX_ED_Value = 8,
        CX_ED_Error = 16,
        CX_ED_All = 31,

        CX_ED_TypeValue = CX_ED_Type | CX_ED_Value,
        CX_ED_TypeInstantiation = CX_ED_Type | CX_ED_Instantiation,
        CX_ED_ValueInstantiation = CX_ED_Value | CX_ED_Instantiation,
        CX_ED_TypeValueInstantiation = CX_ED_Type | CX_ED_Value | CX_ED_Instantiation,
    }
}
