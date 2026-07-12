// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class SubstNonTypeTemplateParmPackExpr : Expr
{
    private ValueLazy<SubstNonTypeTemplateParmPackExpr, TemplateArgument> _argumentPack;
    private ValueLazy<SubstNonTypeTemplateParmPackExpr, NonTypeTemplateParmDecl> _parameterPack;

    internal unsafe SubstNonTypeTemplateParmPackExpr(CXCursor handle) : base(handle, CXCursor_DeclRefExpr, CX_StmtClass_SubstNonTypeTemplateParmPackExpr)
    {
        Debug.Assert(NumChildren is 0);

        _argumentPack = new ValueLazy<SubstNonTypeTemplateParmPackExpr, TemplateArgument>(&ArgumentPackFactory);
        _parameterPack = new ValueLazy<SubstNonTypeTemplateParmPackExpr, NonTypeTemplateParmDecl>(&ParameterPackFactory);
    }

    public TemplateArgument ArgumentPack => _argumentPack.GetValue(this);

    public NonTypeTemplateParmDecl Parameter => _parameterPack.GetValue(this);

    private static unsafe NonTypeTemplateParmDecl ParameterPackFactory(SubstNonTypeTemplateParmPackExpr self) => self.TranslationUnit.GetOrCreate<NonTypeTemplateParmDecl>(self.Handle.Referenced);

    private static unsafe TemplateArgument ArgumentPackFactory(SubstNonTypeTemplateParmPackExpr self) => self.TranslationUnit.GetOrCreate(self.Handle.GetTemplateArgument(0));
}
