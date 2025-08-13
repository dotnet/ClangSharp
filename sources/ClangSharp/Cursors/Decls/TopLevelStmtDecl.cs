// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class TopLevelStmtDecl : Decl
{
    private readonly ValueLazy<LabelStmt> _stmt;

    internal TopLevelStmtDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_TopLevelStmt)
    {
        _stmt = new ValueLazy<LabelStmt>(() => TranslationUnit.GetOrCreate<LabelStmt>(Handle.GetExpr(0)));
    }

    public LabelStmt Stmt => _stmt.Value;
}
