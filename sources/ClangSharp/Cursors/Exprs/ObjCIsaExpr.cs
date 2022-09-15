// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCIsaExpr : Expr
{
    internal ObjCIsaExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_MemberRefExpr, CX_StmtClass.CX_StmtClass_ObjCIsaExpr)
    {
        Debug.Assert(NumChildren is 1);
    }

    public Expr Base => (Expr)Children[0];

    public bool IsArrow => Handle.IsArrow;
}
