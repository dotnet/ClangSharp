// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentAddressSpaceType : Type
    {
        private readonly Lazy<Expr> _addrSpaceExpr;
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Type> _pointeeType;

        internal DependentAddressSpaceType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_DependentAddressSpace)
        {
            _addrSpaceExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.AddrSpaceExpr));
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _pointeeType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.PointeeType));
        }

        public Expr AddrSpaceExpr => _addrSpaceExpr.Value;

        public bool IsSugared => Handle.IsSugared;

        public Type PointeeType => _pointeeType.Value;

        public Type Desugar() => _desugaredType.Value;
    }
}
