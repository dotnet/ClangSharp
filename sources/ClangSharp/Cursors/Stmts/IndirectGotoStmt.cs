// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class IndirectGotoStmt : Stmt
{
    private ValueLazy<IndirectGotoStmt, LabelDecl> _constantTarget;

    internal unsafe IndirectGotoStmt(CXCursor handle) : base(handle, CXCursor_IndirectGotoStmt, CX_StmtClass_IndirectGotoStmt)
    {
        Debug.Assert(NumChildren is 1);
        _constantTarget = new ValueLazy<IndirectGotoStmt, LabelDecl>(&ConstantTargetFactory);
    }

    public LabelDecl ConstantTarget => _constantTarget.GetValue(this);

    public Expr Target => (Expr)Children[0];

    private static unsafe LabelDecl ConstantTargetFactory(IndirectGotoStmt self) => self.TranslationUnit.GetOrCreate<LabelDecl>(self.Handle.Referenced);
}
