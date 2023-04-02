// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class SubstNonTypeTemplateParmPackExpr : Expr
{
    private readonly Lazy<TemplateArgument> _argumentPack;
    private readonly Lazy<NonTypeTemplateParmDecl> _parameterPack;

    internal SubstNonTypeTemplateParmPackExpr(CXCursor handle) : base(handle, CXCursor_DeclRefExpr, CX_StmtClass_SubstNonTypeTemplateParmPackExpr)
    {
        Debug.Assert(NumChildren is 0);

        _argumentPack = new Lazy<TemplateArgument>(() => TranslationUnit.GetOrCreate(Handle.GetTemplateArgument(0)));
        _parameterPack = new Lazy<NonTypeTemplateParmDecl>(() => TranslationUnit.GetOrCreate<NonTypeTemplateParmDecl>(Handle.Referenced));
    }

    public TemplateArgument ArgumentPack => _argumentPack.Value;

    public NonTypeTemplateParmDecl Parameter => _parameterPack.Value;
}
