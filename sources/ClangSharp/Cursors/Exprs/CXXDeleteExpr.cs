// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXDeleteExpr : Expr
    {
        private Lazy<Expr> _argument;
        private Lazy<FunctionDecl> _operatorDelete;

        internal CXXDeleteExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXDeleteExpr, CX_StmtClass.CX_StmtClass_CXXDeleteExpr)
        {
            _argument = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
            _operatorDelete = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.Referenced));
        }

        public Expr Argument => _argument.Value;

        public FunctionDecl OperatorDelete => _operatorDelete.Value;
    }
}
