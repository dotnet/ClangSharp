// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StaticAssertDecl : Decl
    {
        private readonly Lazy<Expr> _assertExpr;
        private readonly Lazy<StringLiteral> _message;

        internal StaticAssertDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_StaticAssert, CX_DeclKind.CX_DeclKind_StaticAssert)
        {
            _assertExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
            _message = new Lazy<StringLiteral>(() => TranslationUnit.GetOrCreate<StringLiteral>(Handle.GetExpr(1)));
        }

        public Expr AssertExpr => _assertExpr.Value;

        public StringLiteral Message => _message.Value;
    }
}
