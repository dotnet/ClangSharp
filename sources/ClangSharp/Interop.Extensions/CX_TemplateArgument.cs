// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;

namespace ClangSharp.Interop
{
    public unsafe partial struct CX_TemplateArgument : IDisposable
    {
        public CXCursor AsDecl => clangsharp.TemplateArgument_getAsDecl(this);

        public CXCursor AsExpr => clangsharp.TemplateArgument_getAsExpr(this);

        public long AsIntegral => clangsharp.TemplateArgument_getAsIntegral(this);

        public CX_TemplateName AsTemplate => clangsharp.TemplateArgument_getAsTemplate(this);

        public CX_TemplateName AsTemplateOrTemplatePattern => clangsharp.TemplateArgument_getAsTemplateOrTemplatePattern(this);

        public CXType AsType => clangsharp.TemplateArgument_getAsType(this);

        public CX_TemplateArgumentDependence Dependence => clangsharp.TemplateArgument_getDependence(this);

        public CXType IntegralType => clangsharp.TemplateArgument_getIntegralType(this);

        public CXType NonTypeTemplateArgumentType => clangsharp.TemplateArgument_getNonTypeTemplateArgumentType(this);

        public CXType NullPtrType => clangsharp.TemplateArgument_getNullPtrType(this);

        public int NumPackElements => clangsharp.TemplateArgument_getNumPackElements(this);

        public CX_TemplateArgument PackExpansionPattern => clangsharp.TemplateArgument_getPackExpansionPattern(this);

        public CXType ParamTypeForDecl => clangsharp.TemplateArgument_getParamTypeForDecl(this);

        public CX_TemplateArgument GetPackElement(uint i) => clangsharp.TemplateArgument_getPackElement(this, i);

        public void Dispose() => clangsharp.TemplateArgument_dispose(this);
    }
}
