namespace ClangSharp
{
    public unsafe partial struct CXCodeCompleteResults
    {
        [NativeTypeName("CXCompletionResult *")]
        public CXCompletionResult* Results;

        [NativeTypeName("unsigned int")]
        public uint NumResults;
    }
}
