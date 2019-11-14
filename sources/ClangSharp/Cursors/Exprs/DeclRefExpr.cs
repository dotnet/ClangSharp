// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DeclRefExpr : Expr
    {
        private readonly Lazy<ValueDecl> _decl;

        internal DeclRefExpr(CXCursor handle) : base(handle, handle.Kind, CX_StmtClass.CX_StmtClass_DeclRefExpr)
        {
            if ((handle.Kind != CXCursorKind.CXCursor_DeclRefExpr) && (handle.Kind != CXCursorKind.CXCursor_ObjCSelfExpr))
            {
                throw new ArgumentException(nameof(handle));
            }

            _decl = new Lazy<ValueDecl>(() => TranslationUnit.GetOrCreate<ValueDecl>(Handle.Referenced));
        }

        public ValueDecl Decl => _decl.Value;
    }
}
