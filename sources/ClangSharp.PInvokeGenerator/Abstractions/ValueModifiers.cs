// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Abstractions
{
    [Flags]
    internal enum ValueFlags
    {
        None = 0,
        Initializer = 1 << 0,
        Constant = 1 << 1,
        Copy = 1 << 2,
        Array = 1 << 3,
    }
}
