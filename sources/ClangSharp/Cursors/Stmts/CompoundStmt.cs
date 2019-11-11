// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System.Collections.Generic;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CompoundStmt : Stmt
    {
        public CompoundStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CompoundStmt)
        {
        }

        public IReadOnlyList<Stmt> Body => Children;
    }
}
