// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXNoexceptExpr : Expr
    {
        private readonly Lazy<Expr> _operand;

        internal CXXNoexceptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryExpr, CX_StmtClass.CX_StmtClass_CXXNoexceptExpr)
        {
            _operand = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
        }

        public Expr Operand => _operand.Value;
    }
}
