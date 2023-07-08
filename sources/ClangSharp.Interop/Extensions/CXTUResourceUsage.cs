// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangSharp.Interop;

public partial struct CXTUResourceUsage : IDisposable, IReadOnlyCollection<CXTUResourceUsageEntry>
{
    public unsafe CXTUResourceUsageEntry this[uint index] => entries[index];

    public readonly int Count => (int)numEntries;

    public readonly void Dispose() => clang.disposeCXTUResourceUsage(this);

    public IEnumerator<CXTUResourceUsageEntry> GetEnumerator()
    {
        var count = (uint)Count;

        for (var index = 0u; index < count; index++)
        {
            yield return this[index];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
