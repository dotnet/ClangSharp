using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CXSourceLocation : IEquatable<CXSourceLocation>
    {
        public static CXSourceLocation Null => clang.getNullLocation();

        public bool IsFromMainFile => clang.Location_isFromMainFile(this) != 0;

        public bool IsInSystemHeader => clang.Location_isInSystemHeader(this) != 0;

        public static bool operator ==(CXSourceLocation left, CXSourceLocation right) => clang.equalLocations(left, right) != 0;

        public static bool operator !=(CXSourceLocation left, CXSourceLocation right) => clang.equalLocations(left, right) == 0;

        public override bool Equals(object obj) => (obj is CXSourceLocation other) && Equals(other);

        public bool Equals(CXSourceLocation other) => this == other;

        public void GetExpansionLocation(out CXFile file, out uint line, out uint column, out uint offset)
        {
            fixed (CXFile* pFile = &file)
            fixed (uint* pLine = &line)
            fixed (uint* pColumn = &column)
            fixed (uint* pOffset = &offset)
            {
                clang.getExpansionLocation(this, (void**)pFile, pLine, pColumn, pOffset);
            }
        }

        public void GetFileLocation(out CXFile file, out uint line, out uint column, out uint offset)
        {
            fixed (CXFile* pFile = &file)
            fixed (uint* pLine = &line)
            fixed (uint* pColumn = &column)
            fixed (uint* pOffset = &offset)
            {
                clang.getFileLocation(this, (void**)pFile, pLine, pColumn, pOffset);
            }
        }

        public override int GetHashCode() => HashCode.Combine(ptr_data, int_data);

        public void GetInstantiationLocation(out CXFile file, out uint line, out uint column, out uint offset)
        {
            fixed (CXFile* pFile = &file)
            fixed (uint* pLine = &line)
            fixed (uint* pColumn = &column)
            fixed (uint* pOffset = &offset)
            {
                clang.getInstantiationLocation(this, (void**)pFile, pLine, pColumn, pOffset);
            }
        }

        public void GetPresumedLocation(out CXString fileName, out uint line, out uint column)
        {
            fixed (CXString* pFileName = &fileName)
            fixed (uint* pLine = &line)
            fixed (uint* pColumn = &column)
            {
                clang.getPresumedLocation(this, pFileName, pLine, pColumn);
            }
        }

        public void GetSpellingLocation(out CXFile file, out uint line, out uint column, out uint offset)
        {
            fixed (CXFile* pFile = &file)
            fixed (uint* pLine = &line)
            fixed (uint* pColumn = &column)
            fixed (uint* pOffset = &offset)
            {
                clang.getSpellingLocation(this, (void**)pFile, pLine, pColumn, pOffset);
            }
        }

        public override string ToString()
        {
            GetFileLocation(out var file, out var line, out var column, out _);
            return $"Line {line}, Column {column} in {file}";
        }
    }
}
