// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_VariableCaptureKind
{
    CX_VCK_Invalid,
    CX_VCK_This = 1,
    CX_VCK_ByRef = 2,
    CX_VCK_ByCopy = 3,
    CX_VCK_VLAType = 4,
}
