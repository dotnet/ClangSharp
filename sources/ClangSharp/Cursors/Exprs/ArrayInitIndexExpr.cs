// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArrayInitIndexExpr : Expr
    {
        internal ArrayInitIndexExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr)
        {
            Debug.Assert(NumChildren is 0);
        }
    }
}
