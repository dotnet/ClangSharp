// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class CXXOperatorCallExpr : CallExpr
{
    internal CXXOperatorCallExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr)
    {
    }

    public CX_OverloadedOperatorKind Operator => Handle.OverloadedOperatorKind;
}
