namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXTypeLayoutError
    {
        CXTypeLayoutError_Invalid = -1,
        CXTypeLayoutError_Incomplete = -2,
        CXTypeLayoutError_Dependent = -3,
        CXTypeLayoutError_NotConstantSize = -4,
        CXTypeLayoutError_InvalidFieldName = -5,
    }
}
