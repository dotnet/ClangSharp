// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Abstractions
{
    [Flags]
    public enum StructFlags
    {
        IsUnsafe = 1 << 0,
        HasVtbl = 1 << 1,
        IsUnion = 1 << 2,
    }
}
