using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXSourceRangeList : IDisposable, IReadOnlyCollection<CXSourceRange>
    {
        public unsafe CXSourceRange this[uint index] => ranges[index];

        public int Count => (int)count;

        public void Dispose()
        {
            fixed (CXSourceRangeList* pThis = &this)
            {
                clang.disposeSourceRangeList(pThis);
            }
        }

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
