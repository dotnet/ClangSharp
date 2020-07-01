// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class GotoStmt : Stmt
    {
        private readonly Lazy<LabelDecl> _label;

        internal GotoStmt(CXCursor handle) : base(handle, CXCursorKind.CXCursor_GotoStmt, CX_StmtClass.CX_StmtClass_GotoStmt)
        {
            _label = new Lazy<LabelDecl>(() => TranslationUnit.GetOrCreate<LabelDecl>(Handle.Referenced));
        }
    }
}
