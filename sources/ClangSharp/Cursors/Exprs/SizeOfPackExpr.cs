// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class SizeOfPackExpr : Expr
    {
        private readonly Lazy<NamedDecl> _pack;

        internal SizeOfPackExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_SizeOfPackExpr, CX_StmtClass.CX_StmtClass_SizeOfPackExpr)
        {
            _pack = new Lazy<NamedDecl>(() => TranslationUnit.GetOrCreate<NamedDecl>(Handle.Referenced));
        }

        public NamedDecl Pack => _pack.Value;
    }
}
