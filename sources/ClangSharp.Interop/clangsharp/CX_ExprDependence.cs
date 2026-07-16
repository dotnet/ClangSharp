// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

using System;

namespace ClangSharp.Interop;

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
    CX_ED_TypeValue = 12,
    CX_ED_TypeInstantiation = 6,
    CX_ED_ValueInstantiation = 10,
    CX_ED_TypeValueInstantiation = 14,
    CX_ED_ErrorDependent = 26,
}
