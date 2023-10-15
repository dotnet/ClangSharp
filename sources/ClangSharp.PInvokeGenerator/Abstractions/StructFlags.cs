// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Abstractions;

[Flags]
internal enum StructFlags
{
    None = 0,
    Unsafe = 1 << 0,
    Vtbl = 1 << 1,
    Union = 1 << 2,
    Nested = 1 << 3,
}
