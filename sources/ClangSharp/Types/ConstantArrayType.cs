// Copyright (c) Microsoft and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class ConstantArrayType : ArrayType
    {
        private readonly Lazy<Expr> _sizeExpr;

        internal ConstantArrayType(CXType handle) : base(handle, CXTypeKind.CXType_ConstantArray, CX_TypeClass.CX_TypeClass_ConstantArray)
        {
            _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SizeExpr));
        }

        public long Size => Handle.ArraySize;

        public Expr SizeExpr => _sizeExpr.Value;
    }
}
