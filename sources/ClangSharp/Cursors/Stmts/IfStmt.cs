// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class IfStmt : Stmt
{
    internal IfStmt(CXCursor handle) : base(handle, CXCursor_IfStmt, CX_StmtClass_IfStmt)
    {
        Debug.Assert(NumChildren is >= 2 and <= 4);
    }

    public Expr Cond => (Expr)Children[CondOffset];

    public VarDecl? ConditionVariable => (VarDecl?)ConditionVariableDeclStmt?.SingleDecl;

    public DeclStmt? ConditionVariableDeclStmt => HasVarStorage ? (DeclStmt)Children[VarOffset] : null;

    public Stmt? Else => HasElseStorage ? Children[ElseOffset] : null;

    public bool HasElseStorage => Handle.HasElseStorage;

    public bool HasInitStorage => Handle.HasInitStorage;

    public bool HasVarStorage => Handle.HasVarStorage;

    public Stmt? Init => HasInitStorage ? Children[InitOffset] : null;

    public bool IsConstexpr => Handle.IsConstexpr;

    public bool IsObjcAvailabilityCheck => Cond is ObjCAvailabilityCheckExpr;

    public Stmt Then => Children[ThenOffset];

    private int CondOffset => VarOffset + (HasVarStorage ? 1 : 0);

    private int ElseOffset => CondOffset + 2;

    private static int InitOffset => 0;

    private int ThenOffset => CondOffset + 1;

    private int VarOffset => InitOffset + (HasInitStorage ? 1 : 0);
}
