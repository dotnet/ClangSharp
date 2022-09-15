// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Interop;

public unsafe partial struct CX_TemplateArgumentLoc
{
    public CX_TemplateArgument Argument => clangsharp.TemplateArgumentLoc_getArgument(this);

    public CXSourceLocation Location => clangsharp.TemplateArgumentLoc_getLocation(this);

    public CXCursor SourceDeclExpression => clangsharp.TemplateArgumentLoc_getSourceDeclExpression(this);

    public CXCursor SourceExpression => clangsharp.TemplateArgumentLoc_getSourceExpression(this);

    public CXCursor SourceIntegralExpression => clangsharp.TemplateArgumentLoc_getSourceIntegralExpression(this);

    public CXCursor SourceNullPtrExpression => clangsharp.TemplateArgumentLoc_getSourceNullPtrExpression(this);

    public CXSourceRange SourceRange => clangsharp.TemplateArgumentLoc_getSourceRange(this);
}
