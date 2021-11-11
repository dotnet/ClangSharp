// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class Index : IDisposable, IEquatable<Index>
    {
        private bool _isDisposed;

        private Index(CXIndex handle)
        {
            Handle = handle;
        }

        ~Index()
        {
            Dispose(isDisposing: false);
        }

        public CXIndex Handle { get; }

        public static bool operator ==(Index left, Index right) => (left is object) ? ((right is object) && (left.Handle == right.Handle)) : (right is null);

        public static bool operator !=(Index left, Index right) => (left is object) ? ((right is null) || (left.Handle != right.Handle)) : (right is object);

        public static Index Create(bool excludeDeclarationsFromPch = false, bool displayDiagnostics = false)
        {
            var handle = CXIndex.Create(excludeDeclarationsFromPch, displayDiagnostics);
            return new Index(handle);
        }

        public void Dispose()
        {
            Dispose(isDisposing: true);
            GC.SuppressFinalize(this);
        }

        public override bool Equals(object obj) => (obj is Index other) && Equals(other);

        public bool Equals(Index other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        private void Dispose(bool isDisposing)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;

            if (Handle != default)
            {
                Handle.Dispose();
            }
        }
    }
}
