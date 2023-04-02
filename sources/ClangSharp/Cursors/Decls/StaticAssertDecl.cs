// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class StaticAssertDecl : Decl
{
    private readonly Lazy<Expr> _assertExpr;
    private readonly Lazy<StringLiteral> _message;

    internal StaticAssertDecl(CXCursor handle) : base(handle, CXCursor_StaticAssert, CX_DeclKind_StaticAssert)
    {
        _assertExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
        _message = new Lazy<StringLiteral>(() => TranslationUnit.GetOrCreate<StringLiteral>(Handle.GetExpr(1)));
    }

    public Expr AssertExpr => _assertExpr.Value;

    public StringLiteral Message => _message.Value;
}
