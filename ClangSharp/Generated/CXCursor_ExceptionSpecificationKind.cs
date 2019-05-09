namespace ClangSharp
{
    public enum CXCursor_ExceptionSpecificationKind
    {
        CXCursor_ExceptionSpecificationKind_None = 0,
        CXCursor_ExceptionSpecificationKind_DynamicNone = 1,
        CXCursor_ExceptionSpecificationKind_Dynamic = 2,
        CXCursor_ExceptionSpecificationKind_MSAny = 3,
        CXCursor_ExceptionSpecificationKind_BasicNoexcept = 4,
        CXCursor_ExceptionSpecificationKind_ComputedNoexcept = 5,
        CXCursor_ExceptionSpecificationKind_Unevaluated = 6,
        CXCursor_ExceptionSpecificationKind_Uninstantiated = 7,
        CXCursor_ExceptionSpecificationKind_Unparsed = 8,
    }
}
