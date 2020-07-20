// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXUuidofExpr : Expr
    {
        private readonly Lazy<Expr> _exprOperand;
        private readonly Lazy<Type> _typeOperand;

        internal CXXUuidofExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_CXXUuidofExpr)
        {
            _exprOperand = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.SubExpr));
            _typeOperand = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(handle.TypeOperand));
        }

        public Expr ExprOperand => _exprOperand.Value;

        public bool IsTypeOperand => Handle.IsTypeOperand;

        public Type TypeOperand => _typeOperand.Value;
    }
}
