// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LabelStmt : ValueStmt
    {
        private readonly Lazy<LabelDecl> _decl;
        private readonly Lazy<Stmt> _subStmt;

        internal LabelStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_LabelStmt, CX_StmtClass.CX_StmtClass_LabelStmt)
        {
            _decl = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
            _subStmt = new Lazy<Stmt>(() => TranslationUnit.GetOrCreate<Stmt>(Handle.SubStmt));
        }

        public LabelDecl Decl => _decl.Value;

        public Stmt SubStmt => _subStmt.Value;
    }
}
