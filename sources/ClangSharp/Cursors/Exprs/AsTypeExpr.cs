// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class AsTypeExpr : Expr
    {
        internal AsTypeExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_AsTypeExpr)
        {
            Debug.Assert(NumChildren is 1);
        }

        public Expr SrcExpr => (Expr)Children[0];
    }
}
