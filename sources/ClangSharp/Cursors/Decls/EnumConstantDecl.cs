// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
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

        public ulong UnsignedInitVal => Handle.EnumConstantDeclUnsignedValue;

        public bool IsNegative => Handle.IsNegative;

        public bool IsNonNegative => Handle.IsNonNegative;

        public bool IsSigned => Handle.IsSigned;

        public bool IsStrictlyPositive => Handle.IsStrictlyPositive;

        public bool IsUnsigned => Handle.IsUnsigned;
    }
}
