// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PackExpansionExpr : Expr
    {
        internal PackExpansionExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_PackExpansionExpr, CX_StmtClass.CX_StmtClass_PackExpansionExpr)
        {
            Debug.Assert(NumChildren is 1);
        }

        public Expr Pattern => (Expr)Children[0];
    }
}
