// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public partial struct CXSourceRange : IEquatable<CXSourceRange>
    {
        public static CXSourceRange Null => clang.getNullRange();

        public CXSourceLocation End => clang.getRangeEnd(this);

        public bool IsNull => clang.Range_isNull(this) != 0;

        public CXSourceLocation Start => clang.getRangeStart(this);

        public static bool operator ==(CXSourceRange left, CXSourceRange right) => Equals(left, right);

        public static bool operator !=(CXSourceRange left, CXSourceRange right) => !Equals(left, right);

        public static bool Equals(CXSourceRange range1, CXSourceRange range2) => clang.equalRanges(range1, range2) != 0;

        public static CXSourceRange Create(CXSourceLocation begin, CXSourceLocation end) => clang.getRange(begin, end);

        public override bool Equals(object obj) => (obj is CXSourceRange other) && Equals(other);

        public bool Equals(CXSourceRange other) => this == other;

        public override int GetHashCode() => HashCode.Combine(ptr_data, begin_int_data, end_int_data);

        public override string ToString() => $"{Start} to {End}";
    }
}
