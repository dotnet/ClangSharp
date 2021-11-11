// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DefaultStmt : SwitchCase
    {
        internal DefaultStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DefaultStmt, CX_StmtClass.CX_StmtClass_DefaultStmt)
        {
            Debug.Assert(NumChildren is 1);
        }

        public new Stmt SubStmt => Children[0];
    }
}
