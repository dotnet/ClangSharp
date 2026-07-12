// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class CallExpr : Expr
{
    private readonly LazyList<Expr, Stmt> _args;
    private ValueLazy<CallExpr, Decl> _calleeDecl;

    internal CallExpr(CXCursor handle) : this(handle, CXCursor_CallExpr, CX_StmtClass_CallExpr)
    {
    }

    private protected unsafe CallExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastCallExpr or < CX_StmtClass_FirstCallExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }

        Debug.Assert(NumChildren >= 1);

        _args = LazyList.Create<Expr, Stmt>(_children, skip: 1, take: (int)NumArgs);
        _calleeDecl = new ValueLazy<CallExpr, Decl>(&CalleeDeclFactory);
    }

    public IReadOnlyList<Expr> Args => _args;

    public Expr Callee => (Expr)Children[0];

    public Decl CalleeDecl => _calleeDecl.GetValue(this);

    public FunctionDecl? DirectCallee => CalleeDecl as FunctionDecl;

    public bool IsCallToStdMove => (NumArgs == 1) && (DirectCallee is FunctionDecl fd) && fd.IsInStdNamespace && fd.Name.Equals("move", StringComparison.Ordinal);

    public uint NumArgs => (uint)Handle.NumArguments;

    private static unsafe Decl CalleeDeclFactory(CallExpr self) => self.TranslationUnit.GetOrCreate<Decl>(self.Handle.Referenced);
}
