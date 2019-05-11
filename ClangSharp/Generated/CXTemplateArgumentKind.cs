namespace ClangSharp
{
    public enum CXTemplateArgumentKind
    {
        CXTemplateArgumentKind_Null,
        CXTemplateArgumentKind_Type,
        CXTemplateArgumentKind_Declaration,
        CXTemplateArgumentKind_NullPtr,
        CXTemplateArgumentKind_Integral,
        CXTemplateArgumentKind_Template,
        CXTemplateArgumentKind_TemplateExpansion,
        CXTemplateArgumentKind_Expression,
        CXTemplateArgumentKind_Pack,
        CXTemplateArgumentKind_Invalid,
    }
}
