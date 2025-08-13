// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXTryStmt : Stmt
{
    private readonly LazyList<CXXCatchStmt, Stmt> _handlers;

    internal CXXTryStmt(CXCursor handle) : base(handle, CXCursor_CXXTryStmt, CX_StmtClass_CXXTryStmt)
    {
        Debug.Assert(NumChildren is >= 1);
        _handlers = LazyList.Create<CXXCatchStmt, Stmt>(_children, skip: 1);
    }

    public IReadOnlyList<CXXCatchStmt> Handlers => _handlers;

    public uint NumHandlers => (uint)(Children.Count - 1);

    public CompoundStmt TryBlock => (CompoundStmt)Children[0];
}
