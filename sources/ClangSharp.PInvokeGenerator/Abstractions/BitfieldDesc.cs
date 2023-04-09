// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;

namespace ClangSharp.Abstractions;

public struct BitfieldDesc
{
    public Type TypeBacking { get; set; }

    public List<BitfieldRegion> Regions { get; set; }
}

public struct BitfieldRegion
{
    public string Name { get; set; }

    public long Offset { get; set; }

    public long Length { get; set; }
}
