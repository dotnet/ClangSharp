// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Abstractions;

[Flags]
internal enum EnumFlags
{
    None = 0,
    Nested = 1 << 0,
}
