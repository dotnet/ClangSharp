// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXInheritedCtorInitExpr : Expr
    {
        private readonly Lazy<CXXConstructorDecl> _constructor;

        internal CXXInheritedCtorInitExpr(CXCursor handle) : base(handle, CXCursorKind.CXCursor_CallExpr, CX_StmtClass.CX_StmtClass_CXXInheritedCtorInitExpr)
        {
            _constructor = new Lazy<CXXConstructorDecl>(() => TranslationUnit.GetOrCreate<CXXConstructorDecl>(Handle.Referenced));
        }

        public CXXConstructorDecl Constructor => _constructor.Value;
    }
}
