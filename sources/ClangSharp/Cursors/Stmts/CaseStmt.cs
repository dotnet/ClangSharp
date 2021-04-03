// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
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
}
