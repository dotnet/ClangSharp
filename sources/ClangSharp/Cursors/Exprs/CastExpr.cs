// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class CastExpr : Expr
    {
        private readonly Lazy<NamedDecl> _conversionFunction;
        private readonly Lazy<Expr> _subExpr;
        private readonly Lazy<Expr> _subExprAsWritten;
        private readonly Lazy<FieldDecl> _targetUnionField;

        private protected CastExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
        {
            if ((CX_StmtClass.CX_StmtClass_LastCastExpr < handle.StmtClass) || (handle.StmtClass < CX_StmtClass.CX_StmtClass_FirstCastExpr))
            {
                throw new ArgumentException(nameof(handle));
            }

            _conversionFunction = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.ConversionFunction));
            _subExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
            _subExprAsWritten = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExprAsWritten));
            _targetUnionField = new Lazy<FieldDecl>(() => TranslationUnit.GetOrCreate<FieldDecl>(Handle.TargetUnionField));
        }

        public CX_CastKind CastKind => Handle.CastKind;

        public string CastKindSpelling => Handle.CastKindSpelling;

        public NamedDecl ConversionFunction => _conversionFunction.Value;

        public Expr SubExpr => _subExpr.Value;

        public Expr SubExprAsWritten => _subExprAsWritten.Value;

        public FieldDecl TargetUnionField => _targetUnionField.Value;
    }
}
