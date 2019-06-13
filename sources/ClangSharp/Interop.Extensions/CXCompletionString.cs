namespace ClangSharp.Interop
{
    public unsafe partial struct CXCompletionString
    {
        public CXAvailabilityKind Availability => clang.getCompletionAvailability(this);

        public CXString BriefComment => clang.getCompletionBriefComment(this);

        public uint NumAnnotations => clang.getCompletionNumAnnotations(this);

        public uint NumChunks => clang.getNumCompletionChunks(this);

        public uint Priority => clang.getCompletionPriority(this);

        public CXString GetAnnotation(uint index) => clang.getCompletionAnnotation(this, index);

        public CXCompletionString GetChunkCompletionString(uint index) => (CXCompletionString)clang.getCompletionChunkCompletionString(this, index);

        public CXCompletionChunkKind GetChunkKind(uint index) => clang.getCompletionChunkKind(this, index);

        public CXString GetChunkText(uint index) => clang.getCompletionChunkText(this, index);

        public CXString GetParent(out CXCursorKind kind)
        {
            fixed (CXCursorKind* pKind = &kind)
            {
                return clang.getCompletionParent(this, pKind);
            }
        }
    }
}
