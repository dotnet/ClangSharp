// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class LabelStmt : ValueStmt
    {
        private readonly Lazy<LabelDecl> _decl;

        internal LabelStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_LabelStmt, CX_StmtClass.CX_StmtClass_LabelStmt)
        {
            Debug.Assert(NumChildren is 1);
            _decl = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
        }

        public LabelDecl Decl => _decl.Value;

        public Stmt SubStmt => Children[0];
    }
}
