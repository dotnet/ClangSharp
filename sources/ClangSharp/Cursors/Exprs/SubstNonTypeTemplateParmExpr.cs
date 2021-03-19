// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SubstNonTypeTemplateParmExpr : Expr
    {
        private readonly Lazy<NonTypeTemplateParmDecl> _parameter;

        internal SubstNonTypeTemplateParmExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_DeclRefExpr, CX_StmtClass.CX_StmtClass_SubstNonTypeTemplateParmExpr)
        {
            Debug.Assert(NumChildren is 1);
            _parameter = new Lazy<NonTypeTemplateParmDecl>(() => TranslationUnit.GetOrCreate<NonTypeTemplateParmDecl>(Handle.Referenced));
        }

        public NonTypeTemplateParmDecl Parameter => _parameter.Value;

        public Expr Replacement => (Expr)Children[0];
    }
}
