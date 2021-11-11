// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCAutoreleasePoolStmt : Stmt
    {
        internal ObjCAutoreleasePoolStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAutoreleasePoolStmt, CX_StmtClass.CX_StmtClass_ObjCAutoreleasePoolStmt)
        {
            Debug.Assert(NumChildren is 1);
        }

        public Stmt SubStmt => Children[0];
    }
}
