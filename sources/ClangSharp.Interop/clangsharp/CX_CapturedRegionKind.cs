// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_CapturedRegionKind
{
    CX_CR_Invalid,
    CX_CR_Default,
    CX_CR_ObjCAtFinally,
    CX_CR_OpenMP,
}
