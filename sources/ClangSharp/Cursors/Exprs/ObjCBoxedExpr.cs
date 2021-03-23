// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ObjCBoxedExpr : Expr
    {
        private readonly Lazy<ObjCMethodDecl> _boxingMethod;

        internal ObjCBoxedExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_UnexposedExpr, CX_StmtClass.CX_StmtClass_ObjCBoxedExpr)
        {
            Debug.Assert(NumChildren is 1);
            _boxingMethod = new Lazy<ObjCMethodDecl>(() => TranslationUnit.GetOrCreate<ObjCMethodDecl>(Handle.Referenced));
        }

        public ObjCMethodDecl BoxingMethod => _boxingMethod.Value;

        public Expr SubExpr => (Expr)Children[0];
    }
}
