// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public class DeclaratorDecl : ValueDecl
    {
        private readonly Lazy<Expr> _trailingRequiresClause;

        private protected DeclaratorDecl(CXCursor handle, CXCursorKind expectedCursorKind, CX_DeclKind expectedDeclKind) : base(handle, expectedCursorKind, expectedDeclKind)
        {
            if ((CX_DeclKind.CX_DeclKind_LastDeclarator < handle.DeclKind) || (handle.DeclKind < CX_DeclKind.CX_DeclKind_FirstDeclarator))
            {
                throw new ArgumentException(nameof(handle));
            }

            _trailingRequiresClause = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.TrailingRequiresClause));
        }

        public Expr TrailingRequiresClause => _trailingRequiresClause.Value;
    }
}
