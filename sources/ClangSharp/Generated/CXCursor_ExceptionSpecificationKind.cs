namespace ClangSharp
{
    public enum CXCursor_ExceptionSpecificationKind
    {
        CXCursor_ExceptionSpecificationKind_None,
        CXCursor_ExceptionSpecificationKind_DynamicNone,
        CXCursor_ExceptionSpecificationKind_Dynamic,
        CXCursor_ExceptionSpecificationKind_MSAny,
        CXCursor_ExceptionSpecificationKind_BasicNoexcept,
        CXCursor_ExceptionSpecificationKind_ComputedNoexcept,
        CXCursor_ExceptionSpecificationKind_Unevaluated,
        CXCursor_ExceptionSpecificationKind_Uninstantiated,
        CXCursor_ExceptionSpecificationKind_Unparsed,
    }
}
