// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXTryStmt : Stmt
    {
        private readonly Lazy<IReadOnlyList<CXXCatchStmt>> _handlers;

        internal CXXTryStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CXXTryStmt, CX_StmtClass.CX_StmtClass_CXXTryStmt)
        {
            Debug.Assert(NumChildren is >= 1);
            _handlers = new Lazy<IReadOnlyList<CXXCatchStmt>>(() => Children.Skip(1).Cast<CXXCatchStmt>().ToList());
        }

        public IReadOnlyList<CXXCatchStmt> Handlers => _handlers.Value;

        public uint NumHandlers => (uint)(Children.Count - 1);

        public CompoundStmt TryBlock => (CompoundStmt)Children[0];
    }
}
