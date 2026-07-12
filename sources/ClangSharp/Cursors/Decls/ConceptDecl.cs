// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class ConceptDecl : TemplateDecl, IMergeable<ConceptDecl>
{
    private ValueLazy<ConceptDecl, Expr> _constraintExpr;

    internal unsafe ConceptDecl(CXCursor handle) : base(handle, CXCursor_ConceptDecl, CX_DeclKind_Concept)
    {
        _constraintExpr = new ValueLazy<ConceptDecl, Expr>(&ConstraintExprFactory);
    }

    public new ConceptDecl CanonicalDecl => (ConceptDecl)base.CanonicalDecl;

    public Expr ConstraintExpr => _constraintExpr.GetValue(this);

    public bool IsTypeConcept => Handle.IsTypeConcept;

    private static unsafe Expr ConstraintExprFactory(ConceptDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.ConstraintExpr);
}
