// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class PredefinedExpr : Expr
{
    internal PredefinedExpr(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_PredefinedExpr)
    {
        Debug.Assert(NumChildren is 0 or 1);
    }

    public StringLiteral? FunctionName => (StringLiteral?)Children.SingleOrDefault();
}
