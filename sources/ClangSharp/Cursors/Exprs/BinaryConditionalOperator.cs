// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class BinaryConditionalOperator : AbstractConditionalOperator
    {
        private readonly Lazy<OpaqueValueExpr> _opaqueValue;

        internal BinaryConditionalOperator(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_BinaryConditionalOperator)
        {
            Debug.Assert(NumChildren is 4);
            _opaqueValue = new Lazy<OpaqueValueExpr>(() => TranslationUnit.GetOrCreate<OpaqueValueExpr>(Handle.OpaqueValue));
        }

        public Expr Common => (Expr)Children[0];

        public new Expr Cond => (Expr)Children[1];

        public new Expr FalseExpr => (Expr)Children[3];

        public OpaqueValueExpr OpaqueValue => _opaqueValue.Value;

        public new Expr TrueExpr => (Expr)Children[2];
    }
}
