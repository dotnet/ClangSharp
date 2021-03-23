// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SEHExceptStmt : Stmt
    {
        internal SEHExceptStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SEHExceptStmt, CX_StmtClass.CX_StmtClass_SEHExceptStmt)
        {
            Debug.Assert(NumChildren is 2);
        }

        public Expr FilterExpr => (Expr)Children[0];

        public CompoundStmt Block => (CompoundStmt)Children[1];
    }
}
