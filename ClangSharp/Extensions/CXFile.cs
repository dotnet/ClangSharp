using System;

namespace ClangSharp
{
    public partial struct CXFile : IEquatable<CXFile>
    {
        public CXString Name => clang.getFileName(this);

        public long Time => clang.getFileTime(this);

        public override bool Equals(object obj) => (obj is CXFile other) && Equals(other);

        public bool Equals(CXFile other) => clang.File_isEqual(this, other) != 0;

        public string GetContents(CXTranslationUnit translationUnit, out ulong size) => clang.getFileContents(translationUnit, this, out size);

        public CXSourceLocation GetLocation(CXTranslationUnit translationUnit, uint line, uint column) => clang.getLocation(translationUnit, this, line, column);

        public CXSourceLocation GetLocationForOffset(CXTranslationUnit translationUnit, uint offset) => clang.getLocationForOffset(translationUnit, this, offset);

        public bool IsMultipleIncludeGuarded(CXTranslationUnit translationUnit) => clang.isFileMultipleIncludeGuarded(translationUnit, this) != 0;

        public override string ToString() => Name.ToString();

        public CXString TryGetRealPathName() => clang.File_tryGetRealPathName(this);
    }
}
