// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXFile : IEquatable<CXFile>
    {
        public CXFile(IntPtr handle)
        {
            Handle = handle;
        }

        public IntPtr Handle { get; set; }

        public CXString Name => clang.getFileName(this);

        public long Time => clang.getFileTime(this);

        public static explicit operator CXFile(void* value) => new CXFile((IntPtr)value);

        public static implicit operator void*(CXFile value) => (void*)value.Handle;

        public static bool operator ==(CXFile left, CXFile right) => clang.File_isEqual(left, right) != 0;

        public static bool operator !=(CXFile left, CXFile right) => clang.File_isEqual(left, right) == 0;

        public override bool Equals(object obj) => (obj is CXFile other) && Equals(other);

        public bool Equals(CXFile other) => this == other;

        public override int GetHashCode() => Handle.GetHashCode();

        public override string ToString() => Name.ToString();

        public bool TryGetUniqueId(out CXFileUniqueID id)
        {
            fixed (CXFileUniqueID* pId = &id)
            {
                return clang.getFileUniqueID(this, pId) == 0;
            }
        }

        public CXString TryGetRealPathName() => clang.File_tryGetRealPathName(this);
    }
}
