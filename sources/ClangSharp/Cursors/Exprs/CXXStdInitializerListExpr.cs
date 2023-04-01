// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXStdInitializerListExpr : Expr
{
    internal CXXStdInitializerListExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_CXXStdInitializerListExpr)
    {
        Debug.Assert(NumChildren is 1);
    }

    public Expr SubExpr => (Expr)Children[0];
}
