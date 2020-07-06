// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class StmtExpr : Expr
    {
        private readonly Lazy<CompoundStmt> _subStmt;

        internal StmtExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_StmtExpr, CX_StmtClass.CX_StmtClass_StmtExpr)
        {
            _subStmt = new Lazy<CompoundStmt>(() => TranslationUnit.GetOrCreate<CompoundStmt>(Handle.SubStmt));
        }

        public CompoundStmt SubStmt => _subStmt.Value;
    }
}
