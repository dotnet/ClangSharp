// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed unsafe class TemplateArgumentLoc
{
    private ValueLazy<TemplateArgumentLoc, TemplateArgument> _argument;
    private ValueLazy<TemplateArgumentLoc, Expr> _sourceDeclExpression;
    private ValueLazy<TemplateArgumentLoc, Expr> _sourceExpression;
    private ValueLazy<TemplateArgumentLoc, Expr> _sourceIntegralExpression;
    private ValueLazy<TemplateArgumentLoc, Expr> _sourceNullPtrExpression;
    private ValueLazy<TemplateArgumentLoc, TranslationUnit> _translationUnit;

    internal TemplateArgumentLoc(CX_TemplateArgumentLoc handle)
    {
        Handle = handle;

        _translationUnit = new ValueLazy<TemplateArgumentLoc, TranslationUnit>(&TranslationUnitFactory);

        _argument = new ValueLazy<TemplateArgumentLoc, TemplateArgument>(&ArgumentFactory);
        _sourceDeclExpression = new ValueLazy<TemplateArgumentLoc, Expr>(&SourceDeclExpressionFactory);
        _sourceExpression = new ValueLazy<TemplateArgumentLoc, Expr>(&SourceExpressionFactory);
        _sourceIntegralExpression = new ValueLazy<TemplateArgumentLoc, Expr>(&SourceIntegralExpressionFactory);
        _sourceNullPtrExpression = new ValueLazy<TemplateArgumentLoc, Expr>(&SourceNullPtrExpressionFactory);
    }

    public TemplateArgument Argument => _argument.GetValue(this);

    public CXSourceLocation Location => Handle.Location;

    public CX_TemplateArgumentLoc Handle { get; }

    public Expr SourceDeclExpression => _sourceDeclExpression.GetValue(this);

    public Expr SourceExpression => _sourceExpression.GetValue(this);

    public Expr SourceIntegralExpression => _sourceIntegralExpression.GetValue(this);

    public Expr SourceNullPtrExpression => _sourceNullPtrExpression.GetValue(this);

    public CXSourceRange SourceRange => Handle.SourceRange;

    public CXSourceRange SourceRangeRaw => Handle.SourceRangeRaw;

    public TranslationUnit TranslationUnit => _translationUnit.GetValue(this);

    private static unsafe Expr SourceNullPtrExpressionFactory(TemplateArgumentLoc self) => self._translationUnit.GetValue(self).GetOrCreate<Expr>(self.Handle.SourceNullPtrExpression);

    private static unsafe Expr SourceIntegralExpressionFactory(TemplateArgumentLoc self) => self._translationUnit.GetValue(self).GetOrCreate<Expr>(self.Handle.SourceIntegralExpression);

    private static unsafe Expr SourceExpressionFactory(TemplateArgumentLoc self) => self._translationUnit.GetValue(self).GetOrCreate<Expr>(self.Handle.SourceExpression);

    private static unsafe Expr SourceDeclExpressionFactory(TemplateArgumentLoc self) => self._translationUnit.GetValue(self).GetOrCreate<Expr>(self.Handle.SourceDeclExpression);

    private static unsafe TemplateArgument ArgumentFactory(TemplateArgumentLoc self) => self._translationUnit.GetValue(self).GetOrCreate(self.Handle.Argument);

    private static unsafe TranslationUnit TranslationUnitFactory(TemplateArgumentLoc self) => TranslationUnit.GetOrCreate(self.Handle.tu);
}
