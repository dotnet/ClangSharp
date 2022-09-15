// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCIndirectCopyRestoreExpr : Expr
{
    internal ObjCIndirectCopyRestoreExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ObjCIndirectCopyRestoreExpr)
    {
        Debug.Assert(NumChildren is 1);
    }

    public bool ShouldCopy => Handle.ShouldCopy;

    public Expr SubExpr => (Expr)Children[0];
}
