// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class StaticAssertDecl : Decl
{
    private ValueLazy<StaticAssertDecl, Expr> _assertExpr;
    private ValueLazy<StaticAssertDecl, StringLiteral> _message;

    internal unsafe StaticAssertDecl(CXCursor handle) : base(handle, CXCursor_StaticAssert, CX_DeclKind_StaticAssert)
    {
        _assertExpr = new ValueLazy<StaticAssertDecl, Expr>(&AssertExprFactory);
        _message = new ValueLazy<StaticAssertDecl, StringLiteral>(&MessageFactory);
    }

    public Expr AssertExpr => _assertExpr.GetValue(this);

    public StringLiteral Message => _message.GetValue(this);

    private static unsafe StringLiteral MessageFactory(StaticAssertDecl self) => self.TranslationUnit.GetOrCreate<StringLiteral>(self.Handle.GetExpr(1));

    private static unsafe Expr AssertExprFactory(StaticAssertDecl self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.GetExpr(0));
}
