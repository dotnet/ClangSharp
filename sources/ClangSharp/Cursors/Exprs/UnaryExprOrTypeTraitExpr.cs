// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryExprOrTypeTraitExpr : Expr
    {
        private readonly Lazy<Expr> _argumentExpr;
        private readonly Lazy<Type> _argumentType;

        internal UnaryExprOrTypeTraitExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryExpr, CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr)
        {
            _argumentExpr = new Lazy<Expr>(() => {
                try
                {
                    return TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr);
                }
                catch
                {
                    return null;
                }
            });
            _argumentType = new Lazy<Type>(() => {
                try
                {
                    return TranslationUnit.GetOrCreate<Type>(Handle.ArgumentType);
                }
                catch
                {
                    return null;
                }
            });
        }

        public Expr ArgumentExpr => _argumentExpr.Value;

        public Type ArgumentType => _argumentType.Value;

        public bool IsArgumentType => ArgumentExpr is null;

        public CX_UnaryExprOrTypeTrait Kind => Handle.UnaryExprOrTypeTraitKind;

        public Type TypeOfArgument => IsArgumentType ? ArgumentType : ArgumentExpr.Type;
    }
}
