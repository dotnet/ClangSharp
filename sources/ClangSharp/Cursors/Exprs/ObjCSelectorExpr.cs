// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCSelectorExpr : Expr
    {
        internal ObjCSelectorExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCSelectorExpr, CX_StmtClass.CX_StmtClass_ObjCSelectorExpr)
        {
        }
    }
}
