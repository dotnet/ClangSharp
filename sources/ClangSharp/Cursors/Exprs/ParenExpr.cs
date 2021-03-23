// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ParenExpr : Expr
    {
        internal ParenExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ParenExpr, CX_StmtClass.CX_StmtClass_ParenExpr)
        {
            Debug.Assert(NumChildren is 1);
        }

        public Expr SubExpr => (Expr)Children[0];
    }
}
