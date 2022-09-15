// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class DefaultStmt : SwitchCase
{
    internal DefaultStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DefaultStmt, CX_StmtClass.CX_StmtClass_DefaultStmt)
    {
        Debug.Assert(NumChildren is 1);
    }

    public new Stmt SubStmt => Children[0];
}
