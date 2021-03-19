// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class OffsetOfExpr : Expr
    {
        private readonly Lazy<IReadOnlyList<Expr>> _indexExprs;
        private readonly Lazy<Type> _typeSourceInfoType;

        internal OffsetOfExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_OffsetOfExpr)
        {
            _indexExprs = new Lazy<IReadOnlyList<Expr>>(() => Children.Cast<Expr>().ToList());
            _typeSourceInfoType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
        }

        public IReadOnlyList<Expr> IndexExprs => _indexExprs.Value;

        public uint NumExpressions => NumChildren;

        public Type TypeSourceInfoType => _typeSourceInfoType.Value;
    }
}
