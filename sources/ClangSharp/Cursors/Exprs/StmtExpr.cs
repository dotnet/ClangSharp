// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StmtExpr : Expr
    {
        internal StmtExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_StmtExpr, CX_StmtClass.CX_StmtClass_StmtExpr)
        {
            Debug.Assert(NumChildren is 1);
        }

        public CompoundStmt SubStmt => (CompoundStmt)Children[0];
    }
}
