// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class TemplateArgumentLoc
    {
        private readonly TemplateArgument _argument;
        private readonly Decl _parentDecl;
        private readonly uint _index;

        private readonly Lazy<Expr> _sourceDeclExpression;
        private readonly Lazy<Expr> _sourceExpression;
        private readonly Lazy<Expr> _sourceIntegralExpression;
        private readonly Lazy<Expr> _sourceNullPtrExpression;

        internal TemplateArgumentLoc(Decl parentDecl, uint index)
        {
            _argument = new TemplateArgument(parentDecl, index);
            _parentDecl = parentDecl;
            _index = index;

            _sourceDeclExpression = new Lazy<Expr>(() => _parentDecl.TranslationUnit.GetOrCreate<Expr>(_parentDecl.Handle.GetTemplateArgumentLocSourceDeclExpression(_index)));
            _sourceExpression = new Lazy<Expr>(() => _parentDecl.TranslationUnit.GetOrCreate<Expr>(_parentDecl.Handle.GetTemplateArgumentLocSourceExpression(_index)));
            _sourceIntegralExpression = new Lazy<Expr>(() => _parentDecl.TranslationUnit.GetOrCreate<Expr>(_parentDecl.Handle.GetTemplateArgumentLocSourceIntegralExpression(_index)));
            _sourceNullPtrExpression = new Lazy<Expr>(() => _parentDecl.TranslationUnit.GetOrCreate<Expr>(_parentDecl.Handle.GetTemplateArgumentLocSourceNullPtrExpression(_index)));
        }

        public TemplateArgument Argument => _argument;

        public CXSourceLocation Location => _parentDecl.Handle.GetTemplateArgumentLocLocation(_index);

        public Expr SourceDeclExpression => _sourceDeclExpression.Value;

        public Expr SourceExpression => _sourceExpression.Value;

        public Expr SourceIntegralExpression => _sourceIntegralExpression.Value;

        public Expr SourceNullPtrExpression => _sourceNullPtrExpression.Value;
    }
}
