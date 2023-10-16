// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;

namespace ClangSharp.Abstractions;

internal struct BitfieldDesc
{
    public Type TypeBacking { get; set; }

    public List<BitfieldRegion> Regions { get; set; }
}
