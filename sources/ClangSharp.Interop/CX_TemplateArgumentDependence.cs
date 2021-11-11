// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.
// Ported from https://github.com/microsoft/ClangSharp/blob/main/sources/libClangSharp

using System;

namespace ClangSharp.Interop
{
    [Flags]
    public enum CX_TemplateArgumentDependence
    {
        CX_TAD_None = 0,
        CX_TAD_UnexpandedPack = 1,
        CX_TAD_Instantiation = 2,
        CX_TAD_Dependent = 4,
        CX_TAD_Error = 8,
        CX_TAD_DependentInstantiation = CX_TAD_Dependent | CX_TAD_Instantiation,
        CX_TAD_All = 15,
    }
}
