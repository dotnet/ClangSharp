namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXChildVisitResult
    {
        CXChildVisit_Break = 0,
        CXChildVisit_Continue = 1,
        CXChildVisit_Recurse = 2,
    }
}
