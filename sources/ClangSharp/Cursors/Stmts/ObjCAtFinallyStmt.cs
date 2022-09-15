// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCAtFinallyStmt : Stmt
{
    internal ObjCAtFinallyStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAtFinallyStmt, CX_StmtClass.CX_StmtClass_ObjCAtFinallyStmt)
    {
        Debug.Assert(NumChildren is 1);
    }

    public Stmt FinallyBody => Children[0];
}
