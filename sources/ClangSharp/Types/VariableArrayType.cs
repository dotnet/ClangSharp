// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class VariableArrayType : ArrayType
    {
        private readonly Lazy<Type> _desugaredType;
        private readonly Lazy<Expr> _sizeExpr;

        internal VariableArrayType(CXType handle) : base(handle, CXTypeKind.CXType_VariableArray, CX_TypeClass.CX_TypeClass_VariableArray)
        {
            _desugaredType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.Desugar()));
            _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.SizeExpr));
        }

        public bool IsSugared => Handle.IsSugared;

        public Type Desugar() => _desugaredType.Value;

        public Expr SizeExpr => _sizeExpr.Value;
    }
}
