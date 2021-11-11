// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class DependentSizedArrayType : ArrayType
    {
        private readonly Lazy<Expr> _sizeExpr;

        internal DependentSizedArrayType(CXType handle) : base(handle, CXTypeKind.CXType_DependentSizedArray, CX_TypeClass.CX_TypeClass_DependentSizedArray)
        {
            _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SizeExpr));
        }

        public Expr SizeExpr => _sizeExpr.Value;
    }
}
