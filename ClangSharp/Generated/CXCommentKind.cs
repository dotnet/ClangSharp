namespace ClangSharp
{
    using System;
    using System.Runtime.InteropServices;

    public enum CXCommentKind
    {
        CXComment_Null = 0,
        CXComment_Text = 1,
        CXComment_InlineCommand = 2,
        CXComment_HTMLStartTag = 3,
        CXComment_HTMLEndTag = 4,
        CXComment_Paragraph = 5,
        CXComment_BlockCommand = 6,
        CXComment_ParamCommand = 7,
        CXComment_TParamCommand = 8,
        CXComment_VerbatimBlockCommand = 9,
        CXComment_VerbatimBlockLine = 10,
        CXComment_VerbatimLine = 11,
        CXComment_FullComment = 12,
    }
}
