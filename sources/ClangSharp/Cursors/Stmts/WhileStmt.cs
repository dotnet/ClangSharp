// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class WhileStmt : Stmt
{
    internal WhileStmt(CXCursor handle) : base(handle, CXCursor_WhileStmt, CX_StmtClass_WhileStmt)
    {
        Debug.Assert(NumChildren is 2 or 3);
    }

    public Stmt Body => Children[BodyOffset];

    public Expr Cond => (Expr)Children[CondOffset];

    public bool HasVarStorage => Handle.HasVarStorage;

    public VarDecl? ConditionVariable => (VarDecl?)ConditionVariableDeclStmt?.SingleDecl;

    public DeclStmt? ConditionVariableDeclStmt => HasVarStorage ? (DeclStmt)Children[VarOffset] : null;

    private int BodyOffset => CondOffset + 1;

    private int CondOffset => VarOffset + (HasVarStorage ? 1 : 0);

    private static int VarOffset => 0;
}
