namespace ClangSharp.Interop
{
    public partial struct CXComment
    {
        public CXString BlockCommandComment_CommandName => clang.BlockCommandComment_getCommandName(this);

        public uint BlockCommandComment_NumArgs => clang.BlockCommandComment_getNumArgs(this);

        public CXComment BlockCommandComment_Paragraph => clang.BlockCommandComment_getParagraph(this);

        public CXString FullComment_AsHtml => clang.FullComment_getAsHTML(this);

        public CXString FullComment_AsXml => clang.FullComment_getAsXML(this);

        public uint HtmlStartTag_NumAttrs => clang.HTMLStartTag_getNumAttrs(this);

        public bool HtmlStartTagComment_IsSelfClosing => clang.HTMLStartTagComment_isSelfClosing(this) != 0;

        public CXString HtmlTagComment_AsString => clang.HTMLTagComment_getAsString(this);

        public CXString HtmlTagComment_TagName => clang.HTMLTagComment_getTagName(this);

        public CXString InlineCommandComment_CommandName => clang.InlineCommandComment_getCommandName(this);

        public uint InlineCommandComment_NumArgs => clang.InlineCommandComment_getNumArgs(this);

        public CXCommentInlineCommandRenderKind InlineCommandComment_RenderKind => clang.InlineCommandComment_getRenderKind(this);

        public bool InlineContentComment_HasTrailingNewline => clang.InlineContentComment_hasTrailingNewline(this) != 0;

        public bool IsWhitespace => clang.Comment_isWhitespace(this) != 0;

        public CXCommentKind Kind => clang.Comment_getKind(this);

        public uint NumChildren => clang.Comment_getNumChildren(this);

        public CXCommentParamPassDirection ParamCommandComment_Direction => clang.ParamCommandComment_getDirection(this);

        public bool ParamCommandComment_IsDirectionExplicit => clang.ParamCommandComment_isDirectionExplicit(this) != 0;

        public bool ParamCommandComment_IsParamIndexValid => clang.ParamCommandComment_isParamIndexValid(this) != 0;

        public uint ParamCommandComment_ParamIndex => clang.ParamCommandComment_getParamIndex(this);

        public CXString ParamCommandComment_ParamName => clang.ParamCommandComment_getParamName(this);

        public CXString TextComment_Text => clang.TextComment_getText(this);

        public uint TParamCommandComment_Depth => clang.TParamCommandComment_getDepth(this);

        public CXString TParamCommandComment_ParamName => clang.TParamCommandComment_getParamName(this);

        public bool TParamCommandComment_IsParamPositionValid => clang.TParamCommandComment_isParamPositionValid(this) != 0;

        public CXString VerbatimBlockLineComment_Text => clang.VerbatimBlockLineComment_getText(this);

        public CXString VerbatimLineComment_Text => clang.VerbatimLineComment_getText(this);

        public CXString BlockCommandComment_GetArgText(uint index) => clang.BlockCommandComment_getArgText(this, index);

        public CXComment GetChild(uint index) => clang.Comment_getChild(this, index);

        public CXString HtmlStartTag_GetAttrName(uint index) => clang.HTMLStartTag_getAttrName(this, index);

        public CXString HtmlStartTag_GetAttrValue(uint index) => clang.HTMLStartTag_getAttrValue(this, index);

        public CXString InlineCommandComment_GetArgText(uint index) => clang.InlineCommandComment_getArgText(this, index);

        public uint TParamCommandComment_GetIndex(uint depth) => clang.TParamCommandComment_getIndex(this, depth);
    }
}
