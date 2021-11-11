// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class MSDependentExistsStmt : Stmt
    {
        internal MSDependentExistsStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedStmt, CX_StmtClass.CX_StmtClass_MSDependentExistsStmt)
        {
            Debug.Assert(NumChildren is 1);
        }

        public bool IsIfExists => Handle.IsIfExists;

        public bool IsIfNotExists => !IsIfExists;

        public string Name => Handle.Name.CString;

        public CompoundStmt SubStmt => (CompoundStmt)Children[0];
    }
}
