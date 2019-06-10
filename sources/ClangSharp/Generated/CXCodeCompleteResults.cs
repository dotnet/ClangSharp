namespace ClangSharp
{
    public unsafe partial struct CXCodeCompleteResults
    {
        public CXCompletionResult* Results;

        public uint NumResults;
    }
}
