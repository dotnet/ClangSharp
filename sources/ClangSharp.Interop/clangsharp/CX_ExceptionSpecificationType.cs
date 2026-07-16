// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_ExceptionSpecificationType
{
    CX_EST_Invalid,
    CX_EST_None = 1,
    CX_EST_DynamicNone = 2,
    CX_EST_Dynamic = 3,
    CX_EST_MSAny = 4,
    CX_EST_NoThrow = 5,
    CX_EST_BasicNoexcept = 6,
    CX_EST_DependentNoexcept = 7,
    CX_EST_NoexceptFalse = 8,
    CX_EST_NoexceptTrue = 9,
    CX_EST_Unevaluated = 10,
    CX_EST_Uninstantiated = 11,
    CX_EST_Unparsed = 12,
}
