// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Interop;

public partial struct CXComment
{
    public readonly CXString BlockCommandComment_CommandName => clang.BlockCommandComment_getCommandName(this);

    public readonly uint BlockCommandComment_NumArgs => clang.BlockCommandComment_getNumArgs(this);

    public readonly CXComment BlockCommandComment_Paragraph => clang.BlockCommandComment_getParagraph(this);

    public readonly CXString FullComment_AsHtml => clang.FullComment_getAsHTML(this);

    public readonly CXString FullComment_AsXml => clang.FullComment_getAsXML(this);

    public readonly uint HtmlStartTag_NumAttrs => clang.HTMLStartTag_getNumAttrs(this);

    public readonly bool HtmlStartTagComment_IsSelfClosing => clang.HTMLStartTagComment_isSelfClosing(this) != 0;

    public readonly CXString HtmlTagComment_AsString => clang.HTMLTagComment_getAsString(this);

    public readonly CXString HtmlTagComment_TagName => clang.HTMLTagComment_getTagName(this);

    public readonly CXString InlineCommandComment_CommandName => clang.InlineCommandComment_getCommandName(this);

    public readonly uint InlineCommandComment_NumArgs => clang.InlineCommandComment_getNumArgs(this);

    public readonly CXCommentInlineCommandRenderKind InlineCommandComment_RenderKind => clang.InlineCommandComment_getRenderKind(this);

    public readonly bool InlineContentComment_HasTrailingNewline => clang.InlineContentComment_hasTrailingNewline(this) != 0;

    public readonly bool IsWhitespace => clang.Comment_isWhitespace(this) != 0;

    public readonly CXCommentKind Kind => clang.Comment_getKind(this);

    public readonly uint NumChildren => clang.Comment_getNumChildren(this);

    public readonly CXCommentParamPassDirection ParamCommandComment_Direction => clang.ParamCommandComment_getDirection(this);

    public readonly bool ParamCommandComment_IsDirectionExplicit => clang.ParamCommandComment_isDirectionExplicit(this) != 0;

    public readonly bool ParamCommandComment_IsParamIndexValid => clang.ParamCommandComment_isParamIndexValid(this) != 0;

    public readonly uint ParamCommandComment_ParamIndex => clang.ParamCommandComment_getParamIndex(this);

    public readonly CXString ParamCommandComment_ParamName => clang.ParamCommandComment_getParamName(this);

    public readonly CXString TextComment_Text => clang.TextComment_getText(this);

    public readonly uint TParamCommandComment_Depth => clang.TParamCommandComment_getDepth(this);

    public readonly CXString TParamCommandComment_ParamName => clang.TParamCommandComment_getParamName(this);

    public readonly bool TParamCommandComment_IsParamPositionValid => clang.TParamCommandComment_isParamPositionValid(this) != 0;

    public readonly CXString VerbatimBlockLineComment_Text => clang.VerbatimBlockLineComment_getText(this);

    public readonly CXString VerbatimLineComment_Text => clang.VerbatimLineComment_getText(this);

    public readonly CXString BlockCommandComment_GetArgText(uint index) => clang.BlockCommandComment_getArgText(this, index);

    public readonly CXComment GetChild(uint index) => clang.Comment_getChild(this, index);

    public readonly CXString HtmlStartTag_GetAttrName(uint index) => clang.HTMLStartTag_getAttrName(this, index);

    public readonly CXString HtmlStartTag_GetAttrValue(uint index) => clang.HTMLStartTag_getAttrValue(this, index);

    public readonly CXString InlineCommandComment_GetArgText(uint index) => clang.InlineCommandComment_getArgText(this, index);

    public readonly uint TParamCommandComment_GetIndex(uint depth) => clang.TParamCommandComment_getIndex(this, depth);
}
