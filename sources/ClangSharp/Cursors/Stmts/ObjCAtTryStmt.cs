// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCAtTryStmt : Stmt
{
    private ValueLazy<ObjCAtTryStmt, LazyList<ObjCAtCatchStmt, Stmt>> _catchStmts;
    private ValueLazy<ObjCAtTryStmt, ObjCAtFinallyStmt?> _finallyStmt;

    internal unsafe ObjCAtTryStmt(CXCursor handle) : base(handle, CXCursor_ObjCAtTryStmt, CX_StmtClass_ObjCAtTryStmt)
    {
        Debug.Assert(NumChildren is >= 1);

        _catchStmts = new ValueLazy<ObjCAtTryStmt, LazyList<ObjCAtCatchStmt, Stmt>>(&CatchStmtsFactory);

        _finallyStmt = new ValueLazy<ObjCAtTryStmt, ObjCAtFinallyStmt?>(&FinallyStmtFactory);
    }

    public Stmt Body => Children[0];

    public IReadOnlyList<ObjCAtCatchStmt> CatchStmts => _catchStmts.GetValue(this);

    public ObjCAtFinallyStmt? FinallyStmt => _finallyStmt.GetValue(this);

    public uint NumCatchStmts => (uint)CatchStmts.Count;

    private static unsafe ObjCAtFinallyStmt? FinallyStmtFactory(ObjCAtTryStmt self) {
            var children = self.Children;

            return (children[children.Count - 1] is ObjCAtFinallyStmt finallyStmt) ? finallyStmt : null;
        }

    private static unsafe LazyList<ObjCAtCatchStmt, Stmt> CatchStmtsFactory(ObjCAtTryStmt self) {
            var children = self._children;
            var skipLast = 0;

            if (children[children.Count - 1] is ObjCAtFinallyStmt)
            {
                skipLast++;
            }

            var take = (int)(self.NumChildren - 1 - skipLast);
            return LazyList.Create<ObjCAtCatchStmt, Stmt>(self._children, skip: 1, take);
        }
}
