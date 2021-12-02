// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

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
