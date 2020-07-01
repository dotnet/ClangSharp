// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using System.Linq;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class EnumConstantDecl : ValueDecl, IMergeable<EnumConstantDecl>
    {
        private readonly Lazy<Expr> _initExpr;

        internal EnumConstantDecl(CXCursor handle) : base(handle, CXCursorKind.CXCursor_EnumConstantDecl, CX_DeclKind.CX_DeclKind_EnumConstant)
        {
            _initExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.InitExpr));
        }

        public new EnumConstantDecl CanonicalDecl => (EnumConstantDecl)base.CanonicalDecl;

        public Expr InitExpr => _initExpr.Value;

        public long InitVal => Handle.EnumConstantDeclValue;

        public bool IsNegative => Handle.IsNegative;

        public bool IsNonNegative => Handle.IsNonNegative;

        public bool IsSigned => Handle.IsSigned;

        public bool IsStrictlyPositive => Handle.IsStrictlyPositive;

        public bool IsUnsigned => Handle.IsUnsigned;
    }
}
