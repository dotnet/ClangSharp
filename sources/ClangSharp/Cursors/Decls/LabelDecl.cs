// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_DeclKind;

namespace ClangSharp;

public sealed class LabelDecl : NamedDecl
{
    private ValueLazy<LabelDecl, LabelStmt> _stmt;

    internal unsafe LabelDecl(CXCursor handle) : base(handle, CXCursor_UnexposedDecl, CX_DeclKind_Label)
    {
        _stmt = new ValueLazy<LabelDecl, LabelStmt>(&StmtFactory);
    }

    public LabelStmt Stmt => _stmt.GetValue(this);

    private static unsafe LabelStmt StmtFactory(LabelDecl self) => self.TranslationUnit.GetOrCreate<LabelStmt>(self.Handle.GetExpr(0));
}
