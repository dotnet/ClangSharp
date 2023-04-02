// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class PackExpansionExpr : Expr
{
    internal PackExpansionExpr(CXCursor handle) : base(handle, CXCursor_PackExpansionExpr, CX_StmtClass_PackExpansionExpr)
    {
        Debug.Assert(NumChildren is 1);
    }

    public Expr Pattern => (Expr)Children[0];
}
