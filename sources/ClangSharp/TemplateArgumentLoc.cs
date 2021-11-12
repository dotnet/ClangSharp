// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed unsafe class TemplateArgumentLoc
    {
        private readonly Lazy<TemplateArgument> _argument;
        private readonly Lazy<Expr> _sourceDeclExpression;
        private readonly Lazy<Expr> _sourceExpression;
        private readonly Lazy<Expr> _sourceIntegralExpression;
        private readonly Lazy<Expr> _sourceNullPtrExpression;
        private readonly Lazy<TranslationUnit> _translationUnit;

        internal TemplateArgumentLoc(CX_TemplateArgumentLoc handle)
        {
            Handle = handle;

            _argument = new Lazy<TemplateArgument>(() => _translationUnit.Value.GetOrCreate(Handle.Argument));
            _sourceDeclExpression = new Lazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.SourceDeclExpression));
            _sourceExpression = new Lazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.SourceExpression));
            _sourceIntegralExpression = new Lazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.SourceIntegralExpression));
            _sourceNullPtrExpression = new Lazy<Expr>(() => _translationUnit.Value.GetOrCreate<Expr>(Handle.SourceNullPtrExpression));
            _translationUnit = new Lazy<TranslationUnit>(() => TranslationUnit.GetOrCreate(Handle.tu));
        }

        public TemplateArgument Argument => _argument.Value;

        public CXSourceLocation Location => Handle.Location;

        public CX_TemplateArgumentLoc Handle { get; }

        public Expr SourceDeclExpression => _sourceDeclExpression.Value;

        public Expr SourceExpression => _sourceExpression.Value;

        public Expr SourceIntegralExpression => _sourceIntegralExpression.Value;

        public Expr SourceNullPtrExpression => _sourceNullPtrExpression.Value;

        public CXSourceRange SourceRange => Handle.SourceRange;

        public TranslationUnit TranslationUnit => _translationUnit.Value;
    }
}
