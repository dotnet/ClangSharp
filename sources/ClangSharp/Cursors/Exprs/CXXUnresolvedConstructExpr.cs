// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXUnresolvedConstructExpr : Expr
{
    private readonly LazyList<Expr, Stmt> _args;
    private readonly ValueLazy<Type> _typeAsWritten;

    internal CXXUnresolvedConstructExpr(CXCursor handle) : base(handle, CXCursor_CallExpr, CX_StmtClass_CXXUnresolvedConstructExpr)
    {
        _args = LazyList.Create<Expr, Stmt>(_children);
        _typeAsWritten = new ValueLazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.TypeOperand));
    }

    public IReadOnlyList<Expr> Args => _args;

    public bool IsListInitialization => Handle.IsListInitialization;

    public uint NumArgs => unchecked((uint)Handle.NumArguments);

    public Type TypeAsWritten => _typeAsWritten.Value;
}
