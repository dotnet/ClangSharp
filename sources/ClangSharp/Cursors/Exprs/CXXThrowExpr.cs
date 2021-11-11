// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXThrowExpr : Expr
    {
        internal CXXThrowExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXThrowExpr, CX_StmtClass.CX_StmtClass_CXXThrowExpr)
        {
            Debug.Assert(NumChildren is 1);
        }

        public bool IsThrownVariableInScope => Handle.IsThrownVariableInScope;

        public Expr SubExpr => (Expr)Children[0];
    }
}
