// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class ArrayInitLoopExpr : Expr
{
    internal ArrayInitLoopExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_ArrayInitLoopExpr)
    {
        Debug.Assert(NumChildren is 2);
    }

    public long ArraySize => Handle.ArraySize;

    public OpaqueValueExpr CommonExpr => (OpaqueValueExpr)Children[0];

    public Expr SubExpr => (Expr)Children[1];
}
