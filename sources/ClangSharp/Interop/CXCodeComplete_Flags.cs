namespace ClangSharp.Interop
{
    public enum CXCodeComplete_Flags
    {
        CXCodeComplete_IncludeMacros = 0x01,
        CXCodeComplete_IncludeCodePatterns = 0x02,
        CXCodeComplete_IncludeBriefComments = 0x04,
        CXCodeComplete_SkipPreamble = 0x08,
        CXCodeComplete_IncludeCompletionsWithFixIts = 0x10,
    }
}
