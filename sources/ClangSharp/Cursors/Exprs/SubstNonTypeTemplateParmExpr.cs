// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class SubstNonTypeTemplateParmExpr : Expr
{
    private ValueLazy<SubstNonTypeTemplateParmExpr, NonTypeTemplateParmDecl> _parameter;

    internal unsafe SubstNonTypeTemplateParmExpr(CXCursor handle) : base(handle, CXCursor_DeclRefExpr, CX_StmtClass_SubstNonTypeTemplateParmExpr)
    {
        Debug.Assert(NumChildren is 1);
        _parameter = new ValueLazy<SubstNonTypeTemplateParmExpr, NonTypeTemplateParmDecl>(&ParameterFactory);
    }

    public NonTypeTemplateParmDecl Parameter => _parameter.GetValue(this);

    public Expr Replacement => (Expr)Children[0];

    private static unsafe NonTypeTemplateParmDecl ParameterFactory(SubstNonTypeTemplateParmExpr self) => self.TranslationUnit.GetOrCreate<NonTypeTemplateParmDecl>(self.Handle.Referenced);
}
