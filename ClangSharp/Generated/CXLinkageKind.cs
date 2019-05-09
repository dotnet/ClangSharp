namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXLinkageKind
    {
        CXLinkage_Invalid = 0,
        CXLinkage_NoLinkage = 1,
        CXLinkage_Internal = 2,
        CXLinkage_UniqueExternal = 3,
        CXLinkage_External = 4,
    }
}
