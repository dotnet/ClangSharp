namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXTypeNullabilityKind
    {
        CXTypeNullability_NonNull = 0,
        CXTypeNullability_Nullable = 1,
        CXTypeNullability_Unspecified = 2,
        CXTypeNullability_Invalid = 3,
    }
}
