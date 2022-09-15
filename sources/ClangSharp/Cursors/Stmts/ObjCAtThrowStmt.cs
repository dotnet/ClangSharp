// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCAtThrowStmt : Stmt
{
    internal ObjCAtThrowStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAtThrowStmt, CX_StmtClass.CX_StmtClass_ObjCAtThrowStmt)
    {
        Debug.Assert(NumChildren is 1);
    }

    public Expr ThrowExpr => (Expr)Children[0];
}
