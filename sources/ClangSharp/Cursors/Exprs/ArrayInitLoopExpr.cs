// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArrayInitLoopExpr : Expr
    {
        internal ArrayInitLoopExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ArrayInitLoopExpr)
        {
            Debug.Assert(NumChildren is 2);
        }

        public long ArraySize => Handle.ArraySize;

        public OpaqueValueExpr CommonExpr => (OpaqueValueExpr)Children[0];

        public Expr SubExpr => (Expr)Children[1];
    }
}
