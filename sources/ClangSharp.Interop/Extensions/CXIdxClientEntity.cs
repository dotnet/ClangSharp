// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXIdxClientEntity : IEquatable<CXIdxClientEntity>
    {
        public CXIdxClientEntity(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public static explicit operator CXIdxClientEntity(void* value) => new CXIdxClientEntity((IntPtr)value);

        public static implicit operator void*(CXIdxClientEntity value) => (void*)value.Handle;

        public static bool operator ==(CXIdxClientEntity left, CXIdxClientEntity right) => left.Handle == right.Handle;

        public static bool operator !=(CXIdxClientEntity left, CXIdxClientEntity right) => left.Handle != right.Handle;

        public override bool Equals(object obj) => (obj is CXIdxClientEntity other) && Equals(other);

        public bool Equals(CXIdxClientEntity other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();
    }
}
