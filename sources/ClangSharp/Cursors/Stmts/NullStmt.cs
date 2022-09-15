// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class NullStmt : Stmt
{
    internal NullStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_NullStmt, CX_StmtClass.CX_StmtClass_NullStmt)
    {
        Debug.Assert(NumChildren is 0);
    }

    public bool HasLeadingEmptyMacro => Handle.HasLeadingEmptyMacro;
}
