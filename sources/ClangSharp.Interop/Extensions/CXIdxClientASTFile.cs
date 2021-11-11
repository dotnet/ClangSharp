// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxClientASTFile : IEquatable<CXIdxClientASTFile>
    {
        public CXIdxClientASTFile(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXIdxClientASTFile(void* value) => new CXIdxClientASTFile((IntPtr)value);

        public static implicit operator void*(CXIdxClientASTFile value) => (void*)value.Handle;

        public static bool operator ==(CXIdxClientASTFile left, CXIdxClientASTFile right) => left.Handle == right.Handle;

        public static bool operator !=(CXIdxClientASTFile left, CXIdxClientASTFile right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXIdxClientASTFile other) && Equals(other);

        public bool Equals(CXIdxClientASTFile other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
