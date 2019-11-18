// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXDynamicCastExpr : CXXNamedCastExpr
    {
        internal CXXDynamicCastExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXDynamicCastExpr, CX_StmtClass.CX_StmtClass_CXXDynamicCastExpr)
        {
        }
    }
}
