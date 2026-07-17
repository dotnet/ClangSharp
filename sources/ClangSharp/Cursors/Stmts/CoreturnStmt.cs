// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CoreturnStmt : Stmt
{
    private ValueLazy<CoreturnStmt, Expr> _operand;
    private ValueLazy<CoreturnStmt, Expr> _promiseCall;

    internal unsafe CoreturnStmt(CXCursor handle) : base(handle, CXCursor_UnexposedStmt, CX_StmtClass_CoreturnStmt)
    {
        _operand = new ValueLazy<CoreturnStmt, Expr>(&OperandFactory);
        _promiseCall = new ValueLazy<CoreturnStmt, Expr>(&PromiseCallFactory);
    }

    public bool IsImplicit => Handle.IsImplicit;

    public Expr Operand => _operand.GetValue(this);

    public Expr PromiseCall => _promiseCall.GetValue(this);

    private static Expr OperandFactory(CoreturnStmt self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.Operand);

    private static Expr PromiseCallFactory(CoreturnStmt self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.PromiseCall);
}
