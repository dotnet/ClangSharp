// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXStringSet : IDisposable, IReadOnlyCollection<CXString>
    {
        public CXString this[uint index] => Strings[index];

        int IReadOnlyCollection<CXString>.Count => (int)Count;

        public void Dispose()
        {
            fixed (CXStringSet* pThis = &this)
            {
                clang.disposeStringSet(pThis);
            }
        }

        public IEnumerator<CXString> GetEnumerator()
        {
            var count = Count;

            for (var index = 0u; index < count; index++)
            {
                yield return this[index];
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
