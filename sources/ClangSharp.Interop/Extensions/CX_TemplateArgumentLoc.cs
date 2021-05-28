// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

namespace ClangSharp.Interop
{
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
}
