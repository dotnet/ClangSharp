// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SEHLeaveStmt : Stmt
    {
        internal SEHLeaveStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SEHLeaveStmt, CX_StmtClass.CX_StmtClass_SEHLeaveStmt)
        {
            Debug.Assert(NumChildren is 0);
        }
    }
}
