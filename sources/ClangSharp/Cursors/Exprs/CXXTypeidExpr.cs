// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXTypeidExpr : Expr
    {
        private readonly Lazy<Type> _typeOperand;

        internal CXXTypeidExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXTypeidExpr, CX_StmtClass.CX_StmtClass_CXXTypeidExpr)
        {
            Debug.Assert(NumChildren is 0 or 1);
            _typeOperand = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(handle.TypeOperand));
        }

        public Expr ExprOperand => (Expr)Children.SingleOrDefault();

        public bool IsPotentiallyEvaluated => Handle.IsPotentiallyEvaluated;

        public bool IsTypeOperand => NumChildren is 0;

        public Type TypeOperand => _typeOperand.Value;
    }
}
