// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Abstractions
{
    [Flags]
    public enum EnumFlags
    {
        None = 0,
        Nested = 1 << 0,
    }
}
