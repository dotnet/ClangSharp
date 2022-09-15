// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class CaseStmt : SwitchCase
{
    internal CaseStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CaseStmt, CX_StmtClass.CX_StmtClass_CaseStmt)
    {
        Debug.Assert(NumChildren is 2 or 3);
    }

    public bool CaseStmtIsGNURange => NumChildren is 3;

    public Expr LHS => (Expr)Children[LHSOffset];

    public Expr RHS => CaseStmtIsGNURange ? (Expr)Children[RHSOffset] : null;

    public new Stmt SubStmt => Children[SubStmtOffset];

    private static int LHSOffset => 0;

    private int RHSOffset => LHSOffset + (CaseStmtIsGNURange ? 1 : 0);

    private int SubStmtOffset => RHSOffset + 1;
}
