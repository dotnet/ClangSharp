using System;

namespace ClangSharp
{
    public partial struct CXSourceLocation : IEquatable<CXSourceLocation>
    {
        public static CXSourceLocation Null
            => clang.getNullLocation();

        public bool IsFromMainFile
            => clang.Location_isFromMainFile(this) != 0;

        public bool IsInSystemHeader
            => clang.Location_isInSystemHeader(this) != 0;

        public override bool Equals(object obj)
            => (obj is CXSourceLocation other) && Equals(other);

        public bool Equals(CXSourceLocation other)
            => clang.equalLocations(this, other) != 0;

        public void GetExpansionLocation(out CXFile file, out uint line, out uint column, out uint offset)
            => clang.getExpansionLocation(this, out file, out line, out column, out offset);

        public void GetFileLocation(out CXFile file, out uint line, out uint column, out uint offset)
            => clang.getFileLocation(this, out file, out line, out column, out offset);

        public void GetInstantiationLocation(out CXFile file, out uint line, out uint column, out uint offset)
            => clang.getInstantiationLocation(this, out file, out line, out column, out offset);

        public void GetPresumedLocation(out CXString fileName, out uint line, out uint column)
            => clang.getPresumedLocation(this, out fileName, out line, out column);

        public void GetSpellingLocation(out CXFile file, out uint line, out uint column, out uint offset)
            => clang.getSpellingLocation(this, out file, out line, out column, out offset);
    }
}
