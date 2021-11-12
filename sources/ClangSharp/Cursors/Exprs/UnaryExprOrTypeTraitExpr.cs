// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
            _argumentExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SubExpr));
            _argumentType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ArgumentType));
        }

        public Expr ArgumentExpr => _argumentExpr.Value;

        public Type ArgumentType => _argumentType.Value;

        public bool IsArgumentType => Handle.IsArgumentType;

        public CX_UnaryExprOrTypeTrait Kind => Handle.UnaryExprOrTypeTraitKind;

        public Type TypeOfArgument => IsArgumentType ? ArgumentType : ArgumentExpr.Type;
    }
}
