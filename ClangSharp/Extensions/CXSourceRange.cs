using System;

namespace ClangSharp
{
    public partial struct CXSourceRange : IEquatable<CXSourceRange>
    {
        public CXSourceRange Create(CXSourceLocation begin, CXSourceLocation end) => clang.getRange(begin, end);

        public static CXSourceRange Null => clang.getNullRange();

        public CXSourceLocation End => clang.getRangeEnd(this);

        public CXSourceLocation Start => clang.getRangeStart(this);

        public override bool Equals(object obj) => (obj is CXSourceRange other) && Equals(other);

        public bool Equals(CXSourceRange other) => clang.equalRanges(this, other) != 0;
    }
}
