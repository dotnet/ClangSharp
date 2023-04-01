// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class LambdaExpr : Expr
{
    internal LambdaExpr(CXCursor handle) : base(handle, CXCursor_LambdaExpr, CX_StmtClass_LambdaExpr)
    {
    }
}
