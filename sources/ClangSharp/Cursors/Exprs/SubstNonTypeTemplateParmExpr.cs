// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class SubstNonTypeTemplateParmExpr : Expr
{
    private readonly ValueLazy<NonTypeTemplateParmDecl> _parameter;

    internal SubstNonTypeTemplateParmExpr(CXCursor handle) : base(handle, CXCursor_DeclRefExpr, CX_StmtClass_SubstNonTypeTemplateParmExpr)
    {
        Debug.Assert(NumChildren is 1);
        _parameter = new ValueLazy<NonTypeTemplateParmDecl>(() => TranslationUnit.GetOrCreate<NonTypeTemplateParmDecl>(Handle.Referenced));
    }

    public NonTypeTemplateParmDecl Parameter => _parameter.Value;

    public Expr Replacement => (Expr)Children[0];
}
