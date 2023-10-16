// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Abstractions;

internal struct BitfieldRegion
{
    public string Name { get; set; }

    public long Offset { get; set; }

    public long Length { get; set; }
}
