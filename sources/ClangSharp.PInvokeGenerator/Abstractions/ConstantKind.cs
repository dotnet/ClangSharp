// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Abstractions
{
    [Flags]
    internal enum ConstantKind
    {
        None = 0,
        ReadOnly = 1 << 0,
        Enumerator = 1 << 1,
        PrimitiveConstant = 1 << 2,
        NonPrimitiveConstant = 1 << 3
    }
}
