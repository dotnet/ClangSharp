// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class DoStmt : Stmt
{
    internal DoStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DoStmt, CX_StmtClass.CX_StmtClass_DoStmt)
    {
        Debug.Assert(NumChildren is 2);
    }

    public Stmt Body => Children[0];

    public Expr Cond => (Expr)Children[1];
}
