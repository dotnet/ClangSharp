// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BinaryConditionalOperator : AbstractConditionalOperator
    {
        private readonly Lazy<Expr> _common;
        private readonly Lazy<OpaqueValueExpr> _opaqueValue;

        internal BinaryConditionalOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_BinaryConditionalOperator)
        {
            _common = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.CommonExpr));
            _opaqueValue = new Lazy<OpaqueValueExpr>(() => TranslationUnit.GetOrCreate<OpaqueValueExpr>(Handle.OpaqueValueExpr));
        }

        public Expr Common => _common.Value;

        public OpaqueValueExpr OpaqueValueExpr => _opaqueValue.Value;
    }
}
