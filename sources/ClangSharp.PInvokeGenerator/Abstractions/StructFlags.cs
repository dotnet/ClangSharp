// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Abstractions
{
    [Flags]
    public enum StructFlags
    {
        None = 0,
        Unsafe = 1 << 0,
        Vtbl = 1 << 1,
        Union = 1 << 2,
        Nested = 1 << 3,
    }
}
