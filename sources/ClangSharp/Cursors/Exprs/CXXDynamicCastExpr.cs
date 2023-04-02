// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class CXXDynamicCastExpr : CXXNamedCastExpr
{
    internal CXXDynamicCastExpr(CXCursor handle) : base(handle, CXCursor_CXXDynamicCastExpr, CX_StmtClass_CXXDynamicCastExpr)
    {
    }

    public bool IsAlwaysNull => Handle.IsAlwaysNull;
}
