// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxClientFile : IEquatable<CXIdxClientFile>
    {
        public CXIdxClientFile(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXIdxClientFile(void* value) => new CXIdxClientFile((IntPtr)value);

        public static implicit operator void*(CXIdxClientFile value) => (void*)value.Handle;

        public static bool operator ==(CXIdxClientFile left, CXIdxClientFile right) => left.Handle == right.Handle;

        public static bool operator !=(CXIdxClientFile left, CXIdxClientFile right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXIdxClientFile other) && Equals(other);

        public bool Equals(CXIdxClientFile other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
