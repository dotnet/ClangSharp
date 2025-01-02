namespace ClangSharp.Test
{
    [SourceLocation("ClangUnsavedFile.h", 1, 8)]
    public partial struct MyStruct
    {
        [SourceLocation("ClangUnsavedFile.h", 3, 9)]
        public int r;

        [SourceLocation("ClangUnsavedFile.h", 4, 9)]
        public int g;

        [SourceLocation("ClangUnsavedFile.h", 5, 9)]
        public int b;
    }
}
