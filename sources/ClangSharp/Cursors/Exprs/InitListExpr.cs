// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class InitListExpr : Expr
    {
        internal InitListExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_InitListExpr, CX_StmtClass.CX_StmtClass_InitListExpr)
        {
        }

        public IReadOnlyList<Stmt> Inits => Children;
    }
}
