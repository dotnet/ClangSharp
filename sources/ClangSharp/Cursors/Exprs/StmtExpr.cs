// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class StmtExpr : Expr
{
    internal StmtExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_StmtExpr, CX_StmtClass.CX_StmtClass_StmtExpr)
    {
        Debug.Assert(NumChildren is 1);
    }

    public CompoundStmt SubStmt => (CompoundStmt)Children[0];
}
