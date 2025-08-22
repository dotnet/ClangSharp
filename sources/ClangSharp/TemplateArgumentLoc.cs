// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed unsafe class TemplateArgumentLoc
{
    private readonly ValueLazy<TemplateArgument> _argument;
    private readonly ValueLazy<Expr> _sourceDeclExpression;
    private readonly ValueLazy<Expr> _sourceExpression;
    private readonly ValueLazy<Expr> _sourceIntegralExpression;
    private readonly ValueLazy<Expr> _sourceNullPtrExpression;
    private readonly ValueLazy<TranslationUnit> _translationUnit;

    internal TemplateArgumentLoc(CX_TemplateArgumentLoc handle)
    {
        Handle = handle;

        _translationUnit = new ValueLazy<TranslationUnit>(() => TranslationUnit.GetOrCreate(Handle.tu));

        _argument = new ValueLazy<TemplateArgument>(() => _translationUnit.Value.GetOrCreate(Handle.Argument));
        _sourceDeclExpression = new ValueLazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.SourceDeclExpression));
        _sourceExpression = new ValueLazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.SourceExpression));
        _sourceIntegralExpression = new ValueLazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.SourceIntegralExpression));
        _sourceNullPtrExpression = new ValueLazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.SourceNullPtrExpression));
    }

    public TemplateArgument Argument => _argument.Value;

    public CXSourceLocation Location => Handle.Location;

    public CX_TemplateArgumentLoc Handle { get; }

    public Expr SourceDeclExpression => _sourceDeclExpression.Value;

    public Expr SourceExpression => _sourceExpression.Value;

    public Expr SourceIntegralExpression => _sourceIntegralExpression.Value;

    public Expr SourceNullPtrExpression => _sourceNullPtrExpression.Value;

    public CXSourceRange SourceRange => Handle.SourceRange;

    public CXSourceRange SourceRangeRaw => Handle.SourceRangeRaw;

    public TranslationUnit TranslationUnit => _translationUnit.Value;
}
