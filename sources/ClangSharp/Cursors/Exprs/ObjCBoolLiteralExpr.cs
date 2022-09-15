// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;

namespace ClangSharp;

public sealed class ObjCBoolLiteralExpr : Expr
{
    internal ObjCBoolLiteralExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCBoolLiteralExpr, CX_StmtClass.CX_StmtClass_ObjCBoolLiteralExpr)
    {
    }

    public bool Value => Handle.BoolLiteralValue;
}
