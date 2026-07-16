// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Ported from https://github.com/dotnet/clangsharp/blob/main/sources/libClangSharp

namespace ClangSharp.Interop;

public enum CX_ObjCMessageReceiverKind
{
    CX_OMRK_Invalid,
    CX_OMRK_Class = 1,
    CX_OMRK_Instance = 2,
    CX_OMRK_SuperClass = 3,
    CX_OMRK_SuperInstance = 4,
}
