// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

namespace ClangSharp.Interop;

public unsafe partial struct CX_TemplateArgumentLoc
{
    public readonly CX_TemplateArgument Argument => clangsharp.TemplateArgumentLoc_getArgument(this);

    public readonly CXSourceLocation Location => clangsharp.TemplateArgumentLoc_getLocation(this);

    public readonly CXCursor SourceDeclExpression => clangsharp.TemplateArgumentLoc_getSourceDeclExpression(this);

    public readonly CXCursor SourceExpression => clangsharp.TemplateArgumentLoc_getSourceExpression(this);

    public readonly CXCursor SourceIntegralExpression => clangsharp.TemplateArgumentLoc_getSourceIntegralExpression(this);

    public readonly CXCursor SourceNullPtrExpression => clangsharp.TemplateArgumentLoc_getSourceNullPtrExpression(this);

    public readonly CXSourceRange SourceRange => clangsharp.TemplateArgumentLoc_getSourceRange(this);

    public readonly CXSourceRange SourceRangeRaw => clangsharp.TemplateArgumentLoc_getSourceRangeRaw(this);
}
