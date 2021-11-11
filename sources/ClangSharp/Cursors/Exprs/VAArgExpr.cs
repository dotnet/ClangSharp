// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class VAArgExpr : Expr
    {
        internal VAArgExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_VAArgExpr)
        {
            Debug.Assert(NumChildren is 1);
        }

        public Expr SubExpr => (Expr)Children[0];
    }
}
