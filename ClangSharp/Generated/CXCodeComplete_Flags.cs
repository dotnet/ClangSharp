namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXCodeComplete_Flags
    {
        CXCodeComplete_IncludeMacros = 1,
        CXCodeComplete_IncludeCodePatterns = 2,
        CXCodeComplete_IncludeBriefComments = 4,
        CXCodeComplete_SkipPreamble = 8,
        CXCodeComplete_IncludeCompletionsWithFixIts = 16,
    }
}
