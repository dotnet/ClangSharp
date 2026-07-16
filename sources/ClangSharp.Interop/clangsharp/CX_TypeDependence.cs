// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

using System;

namespace ClangSharp.Interop;

[Flags]
public enum CX_TypeDependence
{
    CX_TD_None = 0,
    CX_TD_UnexpandedPack = 1,
    CX_TD_Instantiation = 2,
    CX_TD_Dependent = 4,
    CX_TD_VariablyModified = 8,
    CX_TD_Error = 16,
    CX_TD_All = 31,
    CX_TD_DependentInstantiation = 6,
}
