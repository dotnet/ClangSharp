// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXForRangeStmt : Stmt
{
    internal CXXForRangeStmt(CXCursor handle) : base(handle, CXCursor_CXXForRangeStmt, CX_StmtClass_CXXForRangeStmt)
    {
        Debug.Assert(NumChildren is 8);
    }

    public DeclStmt BeginStmt => (DeclStmt)Children[2];

    public Stmt Body => Children[7];

    public Expr Cond => (Expr)Children[4];

    public DeclStmt EndStmt => (DeclStmt)Children[3];

    public Expr Inc => (Expr)Children[5];

    public VarDecl LoopVariable
    {
        get
        {
            var loopVariable = (VarDecl?)LoopVarStmt.SingleDecl;
            Debug.Assert(loopVariable is not null);
            return loopVariable!;
        }
    }

    public DeclStmt LoopVarStmt => (DeclStmt)Children[6];

    public Stmt Init => Children[0];

    public Expr RangeInit
    {
        get
        {
            var rangeDecl = (VarDecl?)RangeStmt.SingleDecl;
            Debug.Assert(rangeDecl is not null);
            return rangeDecl!.Init;
        }
    }

    public DeclStmt RangeStmt => (DeclStmt)Children[1];
}
