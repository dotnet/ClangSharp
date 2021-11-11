// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangSharp.Interop
{
    public partial struct CXTUResourceUsage : IDisposable, IReadOnlyCollection<CXTUResourceUsageEntry>
    {
        public unsafe CXTUResourceUsageEntry this[uint index] => entries[index];

        public int Count => (int)numEntries;

        public void Dispose() => clang.disposeCXTUResourceUsage(this);

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
}
