﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace ClangSharp
{
    public partial struct CXDiagnosticSet : IDisposable, IReadOnlyCollection<CXDiagnostic>
    {
        public CXDiagnostic this[uint index] => GetDiagnostic(index);

        public int Count => (int)clang.getNumDiagnosticsInSet(this);

        public void Dispose() => clang.disposeDiagnosticSet(this);

        public CXDiagnostic GetDiagnostic(uint index) => clang.getDiagnosticInSet(this, index);

        public IEnumerator<CXDiagnostic> GetEnumerator()
        {
            var count = (uint)Count;

            for (var index = 0u; index < count; index++)
            {
                yield return GetDiagnostic(index);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
