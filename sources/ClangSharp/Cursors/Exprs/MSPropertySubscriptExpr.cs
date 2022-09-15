// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class MSPropertySubscriptExpr : Expr
{
    internal MSPropertySubscriptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ArraySubscriptExpr, CX_StmtClass.CX_StmtClass_MSPropertySubscriptExpr)
    {
    }
}
