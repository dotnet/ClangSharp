// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ArrayInitIndexExpr : Expr
    {
        internal ArrayInitIndexExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ArrayInitIndexExpr)
        {
            Debug.Assert(NumChildren is 0);
        }
    }
}
