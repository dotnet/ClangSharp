// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXNullPtrLiteralExpr : Expr
    {
        internal CXXNullPtrLiteralExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXNullPtrLiteralExpr, CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr)
        {
            Debug.Assert(NumChildren is 0);
        }
    }
}
