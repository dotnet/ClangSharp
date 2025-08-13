// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCAtTryStmt : Stmt
{
    private readonly ValueLazy<LazyList<ObjCAtCatchStmt, Stmt>> _catchStmts;
    private readonly ValueLazy<ObjCAtFinallyStmt?> _finallyStmt;

    internal ObjCAtTryStmt(CXCursor handle) : base(handle, CXCursor_ObjCAtTryStmt, CX_StmtClass_ObjCAtTryStmt)
    {
        Debug.Assert(NumChildren is >= 1);

        _catchStmts = new ValueLazy<LazyList<ObjCAtCatchStmt, Stmt>>(() => {
            var children = _children;
            var skipLast = 0;

            if (children[children.Count - 1] is ObjCAtFinallyStmt)
            {
                skipLast++;
            }

            var take = (int)(NumChildren - 1 - skipLast);
            return LazyList.Create<ObjCAtCatchStmt, Stmt>(_children, skip: 1, take);
        });

        _finallyStmt = new ValueLazy<ObjCAtFinallyStmt?>(() => {
            var children = Children;

            return (children[children.Count - 1] is ObjCAtFinallyStmt finallyStmt) ? finallyStmt : null;
        });
    }

    public Stmt Body => Children[0];

    public IReadOnlyList<ObjCAtCatchStmt> CatchStmts => _catchStmts.Value;

    public ObjCAtFinallyStmt? FinallyStmt => _finallyStmt.Value;

    public uint NumCatchStmts => (uint)CatchStmts.Count;
}
