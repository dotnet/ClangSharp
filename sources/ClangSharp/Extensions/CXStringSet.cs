using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangSharp
{
    public partial struct CXStringSet : IDisposable, IReadOnlyCollection<CXString>
    {
        public unsafe CXString this[uint index] => ((CXString*)Strings)[index];

        int IReadOnlyCollection<CXString>.Count => (int)Count;

        public void Dispose() => clang.disposeStringSet(ref this);

        public IEnumerator<CXString> GetEnumerator()
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
