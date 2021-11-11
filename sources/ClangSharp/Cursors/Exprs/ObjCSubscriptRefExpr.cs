// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCSubscriptRefExpr : Expr
    {
        private readonly Lazy<ObjCMethodDecl> _atIndexMethodDecl;

        internal ObjCSubscriptRefExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ObjCSubscriptRefExpr)
        {
            Debug.Assert(NumChildren is 2);
            _atIndexMethodDecl = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));
        }

        public ObjCMethodDecl AtIndexMethodDecl => _atIndexMethodDecl.Value;

        public Expr BaseExpr => (Expr)Children[0];

        public Expr KeyExpr => (Expr)Children[1];

        public bool IsArraySubscriptRefExpr => KeyExpr.Type.IsIntegralOrEnumerationType;
    }
}
