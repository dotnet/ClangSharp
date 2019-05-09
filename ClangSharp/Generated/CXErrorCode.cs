namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXErrorCode
    {
        CXError_Success = 0,
        CXError_Failure = 1,
        CXError_Crashed = 2,
        CXError_InvalidArguments = 3,
        CXError_ASTReadError = 4,
    }
}
