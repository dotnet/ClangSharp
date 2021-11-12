// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class OpaqueValueExpr : Expr
    {
        private readonly Lazy<Expr> _sourceExpr;

        internal OpaqueValueExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_OpaqueValueExpr)
        {
            _sourceExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.SubExpr));
        }

        public Expr SourceExpr => _sourceExpr.Value;
    }
}
