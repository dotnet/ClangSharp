// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCAtTryStmt : Stmt
    {
        private readonly Lazy<IReadOnlyList<ObjCAtCatchStmt>> _catchStmts;
        private readonly Lazy<ObjCAtFinallyStmt> _finallyStmt;

        internal ObjCAtTryStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAtTryStmt, CX_StmtClass.CX_StmtClass_ObjCAtTryStmt)
        {
            Debug.Assert(NumChildren is >= 1);

            _catchStmts = new Lazy<IReadOnlyList<ObjCAtCatchStmt>>(() => {
                var children = Children;
                var skipLast = 0;

                if (children.Last() is ObjCAtFinallyStmt) {
                    skipLast++;
                }

                return children.Skip(1).Take((int)(NumChildren - 1 - skipLast)).Cast<ObjCAtCatchStmt>().ToList();
            });

            _finallyStmt = new Lazy<ObjCAtFinallyStmt>(() => {
                var children = Children;

                return children.Last() is ObjCAtFinallyStmt finallyStmt ? finallyStmt : null;
            });
        }

        public Stmt Body => Children[0];

        public IReadOnlyList<ObjCAtCatchStmt> CatchStmts => _catchStmts.Value;

        public ObjCAtFinallyStmt FinallyStmt => _finallyStmt.Value;

        public uint NumCatchStmts => (uint)CatchStmts.Count;
    }
}
