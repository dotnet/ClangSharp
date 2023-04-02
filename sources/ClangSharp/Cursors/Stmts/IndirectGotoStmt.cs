// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class IndirectGotoStmt : Stmt
{
    private readonly Lazy<LabelDecl> _constantTarget;

    internal IndirectGotoStmt(CXCursor handle) : base(handle, CXCursor_IndirectGotoStmt, CX_StmtClass_IndirectGotoStmt)
    {
        Debug.Assert(NumChildren is 1);
        _constantTarget = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
    }

    public LabelDecl ConstantTarget => _constantTarget.Value;

    public Expr Target => (Expr)Children[0];
}
