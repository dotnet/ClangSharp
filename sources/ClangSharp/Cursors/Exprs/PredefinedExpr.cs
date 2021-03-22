// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class PredefinedExpr : Expr
    {
        internal PredefinedExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_PredefinedExpr)
        {
            Debug.Assert(NumChildren is 0 or 1);
        }

        public StringLiteral FunctionName => (StringLiteral)Children.SingleOrDefault();
    }
}
