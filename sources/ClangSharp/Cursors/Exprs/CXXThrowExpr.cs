// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class CXXThrowExpr : Expr
{
    internal CXXThrowExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXThrowExpr, CX_StmtClass.CX_StmtClass_CXXThrowExpr)
    {
        Debug.Assert(NumChildren is 1);
    }

    public bool IsThrownVariableInScope => Handle.IsThrownVariableInScope;

    public Expr SubExpr => (Expr)Children[0];
}
