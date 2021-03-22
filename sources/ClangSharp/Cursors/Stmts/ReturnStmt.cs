// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ReturnStmt : Stmt
    {
        private readonly Lazy<VarDecl> _nrvoCandidate;

        internal ReturnStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ReturnStmt, CX_StmtClass.CX_StmtClass_ReturnStmt)
        {
            Debug.Assert(NumChildren is 0 or 1);

            _nrvoCandidate = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
        }

        public VarDecl NRVOCandidate => _nrvoCandidate.Value;

        public Expr RetValue => NumChildren != 0 ? (Expr)Children[0] : null;
    }
}
