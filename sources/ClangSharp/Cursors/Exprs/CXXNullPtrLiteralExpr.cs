// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXNullPtrLiteralExpr : Expr
    {
        internal CXXNullPtrLiteralExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXNullPtrLiteralExpr, CX_StmtClass.CX_StmtClass_CXXNullPtrLiteralExpr)
        {
            Debug.Assert(NumChildren is 0);
        }
    }
}
