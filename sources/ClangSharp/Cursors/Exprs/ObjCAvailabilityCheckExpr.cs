// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCAvailabilityCheckExpr : Expr
    {
        internal ObjCAvailabilityCheckExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAvailabilityCheckExpr, CX_StmtClass.CX_StmtClass_ObjCAvailabilityCheckExpr)
        {
        }
    }
}
