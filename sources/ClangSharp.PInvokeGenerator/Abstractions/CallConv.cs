// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Abstractions;

internal enum CallConv
{
    Winapi = 1,
    Cdecl = 2,
    StdCall = 3,
    ThisCall = 4,
    FastCall = 5,
    MemberFunction = 6,
}
