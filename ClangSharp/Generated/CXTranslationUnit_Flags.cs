namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXTranslationUnit_Flags
    {
        CXTranslationUnit_None = 0,
        CXTranslationUnit_DetailedPreprocessingRecord = 1,
        CXTranslationUnit_Incomplete = 2,
        CXTranslationUnit_PrecompiledPreamble = 4,
        CXTranslationUnit_CacheCompletionResults = 8,
        CXTranslationUnit_ForSerialization = 16,
        CXTranslationUnit_CXXChainedPCH = 32,
        CXTranslationUnit_SkipFunctionBodies = 64,
        CXTranslationUnit_IncludeBriefCommentsInCodeCompletion = 128,
        CXTranslationUnit_CreatePreambleOnFirstParse = 256,
        CXTranslationUnit_KeepGoing = 512,
        CXTranslationUnit_SingleFileParse = 1024,
        CXTranslationUnit_LimitSkipFunctionBodiesToPreamble = 2048,
        CXTranslationUnit_IncludeAttributedTypes = 4096,
        CXTranslationUnit_VisitImplicitAttributes = 8192,
    }
}
