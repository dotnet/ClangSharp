// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXNoexceptExpr : Expr
    {
        internal CXXNoexceptExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryExpr, CX_StmtClass.CX_StmtClass_CXXNoexceptExpr)
        {
            Debug.Assert(NumChildren is 1);
        }

        public Expr Operand => (Expr)Children[0];

        public bool Value => Handle.BoolLiteralValue;
    }
}
