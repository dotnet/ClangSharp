// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class StmtExpr : Expr
{
    internal StmtExpr(CXCursor handle) : base(handle, CXCursor_StmtExpr, CX_StmtClass_StmtExpr)
    {
        Debug.Assert(NumChildren is 1);
    }

    public CompoundStmt SubStmt => (CompoundStmt)Children[0];
}
