namespace ClangSharp
{
    public enum CXTemplateArgumentKind
    {
        CXTemplateArgumentKind_Null = 0,
        CXTemplateArgumentKind_Type = 1,
        CXTemplateArgumentKind_Declaration = 2,
        CXTemplateArgumentKind_NullPtr = 3,
        CXTemplateArgumentKind_Integral = 4,
        CXTemplateArgumentKind_Template = 5,
        CXTemplateArgumentKind_TemplateExpansion = 6,
        CXTemplateArgumentKind_Expression = 7,
        CXTemplateArgumentKind_Pack = 8,
        CXTemplateArgumentKind_Invalid = 9,
    }
}
