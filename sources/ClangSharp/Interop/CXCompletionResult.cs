namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompletionResult
    {
        [NativeTypeName("enum CXCursorKind")]
        public CXCursorKind CursorKind;

        [NativeTypeName("CXCompletionString")]
        public void* CompletionString;
    }
}
