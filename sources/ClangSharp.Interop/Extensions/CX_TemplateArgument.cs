// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;

namespace ClangSharp.Interop;

public unsafe partial struct CX_TemplateArgument : IDisposable
{
    public readonly CXCursor AsDecl => clangsharp.TemplateArgument_getAsDecl(this);

    public readonly CXCursor AsExpr => clangsharp.TemplateArgument_getAsExpr(this);

    public readonly long AsIntegral => clangsharp.TemplateArgument_getAsIntegral(this);

    public readonly CX_TemplateName AsTemplate => clangsharp.TemplateArgument_getAsTemplate(this);

    public readonly CX_TemplateName AsTemplateOrTemplatePattern => clangsharp.TemplateArgument_getAsTemplateOrTemplatePattern(this);

    public readonly CXType AsType => clangsharp.TemplateArgument_getAsType(this);

    public readonly CX_TemplateArgumentDependence Dependence => clangsharp.TemplateArgument_getDependence(this);

    public readonly CXType IntegralType => clangsharp.TemplateArgument_getIntegralType(this);

    public readonly CXType NonTypeTemplateArgumentType => clangsharp.TemplateArgument_getNonTypeTemplateArgumentType(this);

    public readonly CXType NullPtrType => clangsharp.TemplateArgument_getNullPtrType(this);

    public readonly int NumPackElements => clangsharp.TemplateArgument_getNumPackElements(this);

    public readonly CX_TemplateArgument PackExpansionPattern => clangsharp.TemplateArgument_getPackExpansionPattern(this);

    public readonly CXType ParamTypeForDecl => clangsharp.TemplateArgument_getParamTypeForDecl(this);

    public readonly CX_TemplateArgument GetPackElement(uint i) => clangsharp.TemplateArgument_getPackElement(this, i);

    public readonly void Dispose() => clangsharp.TemplateArgument_dispose(this);
}
