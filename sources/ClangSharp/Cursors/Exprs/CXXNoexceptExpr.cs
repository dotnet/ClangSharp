// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

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
