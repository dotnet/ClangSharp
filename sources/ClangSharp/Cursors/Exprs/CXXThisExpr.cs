// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class CXXThisExpr : Expr
{
    internal CXXThisExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXThisExpr, CX_StmtClass.CX_StmtClass_CXXThisExpr)
    {
    }

    public bool IsImplicit => Handle.IsImplicit;
}
