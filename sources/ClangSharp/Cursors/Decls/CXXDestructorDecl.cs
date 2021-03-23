// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class CXXDestructorDecl : CXXMethodDecl
    {
        private readonly Lazy<FunctionDecl> _operatorDelete;
        private readonly Lazy<Expr> _operatorDeleteThisArg;

        internal CXXDestructorDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_Destructor, CX_DeclKind.CX_DeclKind_CXXDestructor)
        {
            _operatorDelete = new Lazy<FunctionDecl>(() => TranslationUnit.GetOrCreate<FunctionDecl>(Handle.GetSubDecl(0)));
            _operatorDeleteThisArg = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.GetExpr(0)));
        }

        public new CXXDestructorDecl CanonicalDecl => (CXXDestructorDecl)base.CanonicalDecl;

        public Expr OperatorDeleteThisArg => _operatorDeleteThisArg.Value;

        public FunctionDecl OperatorDelete => _operatorDelete.Value;
    }
}
