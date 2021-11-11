// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
