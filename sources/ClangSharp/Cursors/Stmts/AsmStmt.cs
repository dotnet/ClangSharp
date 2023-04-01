// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public class AsmStmt : Stmt
{
    private protected AsmStmt(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass_LastAsmStmt or < CX_StmtClass_FirstAsmStmt)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
    }
}
