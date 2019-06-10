namespace ClangSharp
{
    public unsafe partial struct CXCompletionResult
    {
        public CXCursorKind CursorKind;

        [NativeTypeName("CXCompletionString")]
        public void* CompletionString;
    }
}
