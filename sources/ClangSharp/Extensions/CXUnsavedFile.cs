namespace ClangSharp
{
    public partial struct CXUnsavedFile
    {
        public static CXUnsavedFile Create(string filename, string contents)
        {
            return new CXUnsavedFile()
            {
                Filename = filename,
                Contents = contents,
                Length = (uint)contents.Length
            };
        }
    }
}
