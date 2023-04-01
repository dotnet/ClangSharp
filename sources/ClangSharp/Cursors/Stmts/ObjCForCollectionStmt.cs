// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ObjCForCollectionStmt : Stmt
{
    internal ObjCForCollectionStmt(CXCursor handle) : base(handle, CXCursor_ObjCForCollectionStmt, CX_StmtClass_ObjCForCollectionStmt)
    {
        Debug.Assert(NumChildren is 3);
    }

    public Stmt Body => Children[2];

    public Expr Collection => (Expr)Children[1];

    public Stmt Element => Children[0];
}
