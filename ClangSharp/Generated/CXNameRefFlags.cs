namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXNameRefFlags
    {
        CXNameRange_WantQualifier = 1,
        CXNameRange_WantTemplateArgs = 2,
        CXNameRange_WantSinglePiece = 4,
    }
}
