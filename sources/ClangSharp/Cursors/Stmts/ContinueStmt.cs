// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ContinueStmt : Stmt
{
    internal ContinueStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ContinueStmt, CX_StmtClass.CX_StmtClass_ContinueStmt)
    {
        Debug.Assert(NumChildren is 0);
    }
}
