using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ClangSharp
{
    public partial struct CXSourceRangeList : IDisposable, IReadOnlyCollection<CXSourceRange>
    {
        public unsafe CXSourceRange this[uint index] => Unsafe.Add(ref Unsafe.AsRef<CXSourceRange>(ranges.ToPointer()), (int)index);

        public int Count => (int)count;

        public void Dispose() => clang.disposeSourceRangeList(ref this);

        public IEnumerator<CXSourceRange> GetEnumerator()
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
