// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class SEHFinallyStmt : Stmt
{
    internal SEHFinallyStmt(CXCursor handle) : base(handle, CXCursor_SEHFinallyStmt, CX_StmtClass_SEHFinallyStmt)
    {
        Debug.Assert(NumChildren is 1);
    }

    public CompoundStmt Block => (CompoundStmt)Children[0];
}
