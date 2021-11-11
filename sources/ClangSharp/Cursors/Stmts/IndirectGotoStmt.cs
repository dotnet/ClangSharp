// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class IndirectGotoStmt : Stmt
    {
        private readonly Lazy<LabelDecl> _constantTarget;

        internal IndirectGotoStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_IndirectGotoStmt, CX_StmtClass.CX_StmtClass_IndirectGotoStmt)
        {
            Debug.Assert(NumChildren is 1);
            _constantTarget = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
        }

        public LabelDecl ConstantTarget => _constantTarget.Value;

        public Expr Target => (Expr)Children[0];
    }
}
