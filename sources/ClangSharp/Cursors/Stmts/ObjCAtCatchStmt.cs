// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCAtCatchStmt : Stmt
    {
        private readonly Lazy<VarDecl> _catchParamDecl;

        internal ObjCAtCatchStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_ObjCAtCatchStmt, CX_StmtClass.CX_StmtClass_ObjCAtCatchStmt)
        {
            Debug.Assert(NumChildren is 1);
            _catchParamDecl = new Lazy<VarDecl>(() => TranslationUnit.GetOrCreate<VarDecl>(Handle.Referenced));
        }

        public Stmt CatchBody => Children[0];

        public VarDecl CatchParamDecl => _catchParamDecl.Value;

        public bool HasEllipsis => CatchParamDecl == null;
    }
}
