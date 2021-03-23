// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXOperatorCallExpr : CallExpr
    {
        internal CXXOperatorCallExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXOperatorCallExpr)
        {
        }

        public CX_OverloadedOperatorKind Operator => Handle.OverloadedOperatorKind;
    }
}
