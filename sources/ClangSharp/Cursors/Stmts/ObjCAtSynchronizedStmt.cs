// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCAtSynchronizedStmt : Stmt
{
    internal ObjCAtSynchronizedStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAtSynchronizedStmt, CX_StmtClass.CX_StmtClass_ObjCAtSynchronizedStmt)
    {
        Debug.Assert(NumChildren is 2);
    }

    public CompoundStmt SynchBody => (CompoundStmt)Children[1];

    public Expr SynchExpr => (Expr)Children[0];
}
