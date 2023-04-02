// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class NullStmt : Stmt
{
    internal NullStmt(CXCursor handle) : base(handle, CXCursor_NullStmt, CX_StmtClass_NullStmt)
    {
        Debug.Assert(NumChildren is 0);
    }

    public bool HasLeadingEmptyMacro => Handle.HasLeadingEmptyMacro;
}
