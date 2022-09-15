// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public class CXXNamedCastExpr : ExplicitCastExpr
{
    private protected CXXNamedCastExpr(CXCursor handle, CXCursorKind expectedCursorKind, CX_StmtClass expectedStmtClass) : base(handle, expectedCursorKind, expectedStmtClass)
    {
        if (handle.StmtClass is > CX_StmtClass.CX_StmtClass_LastCXXNamedCastExpr or < CX_StmtClass.CX_StmtClass_FirstCXXNamedCastExpr)
        {
            throw new ArgumentOutOfRangeException(nameof(handle));
        }
    }

    public string CastName => Handle.Name.CString;
}
