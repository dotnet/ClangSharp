namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public partial struct CXCompletionResult
    {
        public CXCursorKind CursorKind;
        public CXCompletionString CompletionString;
    }
}
