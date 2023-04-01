// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ConceptDecl : TemplateDecl, IMergeable<ConceptDecl>
{
    private readonly Lazy<Expr> _constraintExpr;

    internal ConceptDecl(CXCursor handle) : base(handle, CXCursor_ConceptDecl, CX_DeclKind_Concept)
    {
        _constraintExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.ConstraintExpr));
    }

    public new ConceptDecl CanonicalDecl => (ConceptDecl)base.CanonicalDecl;

    public Expr ConstraintExpr => _constraintExpr.Value;

    public bool IsTypeConcept => Handle.IsTypeConcept;
}
