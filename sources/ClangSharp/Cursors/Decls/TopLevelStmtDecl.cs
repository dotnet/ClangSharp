// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CX_DeclKind;
using static ClangSharp.Interop.CXCursorKind;

namespace ClangSharp;

public sealed class TopLevelStmtDecl : Decl
{
    private ValueLazy<TopLevelStmtDecl, LabelStmt> _stmt;

    internal unsafe TopLevelStmtDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_TopLevelStmt)
    {
        _stmt = new ValueLazy<TopLevelStmtDecl, LabelStmt>(&StmtFactory);
    }

    public LabelStmt Stmt => _stmt.GetValue(this);

    private static unsafe LabelStmt StmtFactory(TopLevelStmtDecl self) => self.TranslationUnit.GetOrCreate<LabelStmt>(self.Handle.GetExpr(0));
}
