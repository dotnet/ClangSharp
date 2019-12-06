// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class UnaryExprOrTypeTraitExpr : Expr
    {
        internal UnaryExprOrTypeTraitExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnaryExpr, CX_StmtClass.CX_StmtClass_UnaryExprOrTypeTraitExpr)
        {
        }
    }
}
