// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
