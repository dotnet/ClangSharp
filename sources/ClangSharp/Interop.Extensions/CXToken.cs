// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXToken : IEquatable<CXToken>
    {
        public CXTokenKind Kind => clang.getTokenKind(this);

        public static bool operator ==(CXToken left, CXToken right)
        {
            return (left.int_data[0] == right.int_data[0]) &&
                   (left.int_data[1] == right.int_data[1]) &&
                   (left.int_data[2] == right.int_data[2]) &&
                   (left.int_data[3] == right.int_data[3]) &&
                   (left.ptr_data == right.ptr_data);
        }

        public static bool operator !=(CXToken left, CXToken right)
        {
            return (left.int_data[0] != right.int_data[0]) ||
                   (left.int_data[1] != right.int_data[1]) ||
                   (left.int_data[2] != right.int_data[2]) ||
                   (left.int_data[3] != right.int_data[3]) ||
                   (left.ptr_data != right.ptr_data);
        }

        public override bool Equals(object obj) => (obj is CXSourceRange other) && Equals(other);

        public bool Equals(CXToken other) => this == other;

        public CXSourceRange GetExtent(CXTranslationUnit translationUnit) => clang.getTokenExtent(translationUnit, this);

        public override int GetHashCode() => HashCode.Combine(int_data[0], int_data[1], int_data[2], int_data[3], (IntPtr)ptr_data);

        public CXSourceLocation GetLocation(CXTranslationUnit translationUnit) => clang.getTokenLocation(translationUnit, this);

        public CXString GetSpelling(CXTranslationUnit translationUnit) => clang.getTokenSpelling(translationUnit, this);

        public override string ToString() => "";
    }
}
