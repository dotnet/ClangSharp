// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

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
