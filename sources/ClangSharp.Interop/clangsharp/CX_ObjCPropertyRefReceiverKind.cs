// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_ObjCPropertyRefReceiverKind
{
    CX_OPRK_Invalid,
    CX_OPRK_Object,
    CX_OPRK_Super,
    CX_OPRK_Class,
}
