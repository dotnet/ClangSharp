// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_ConstructionKind
{
    CX_CK_Invalid,
    CX_CK_Complete,
    CX_CK_NonVirtualBase,
    CX_CK_VirtualBase,
    CX_CK_Delegating,
}
