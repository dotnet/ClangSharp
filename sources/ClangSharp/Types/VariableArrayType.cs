// Copyright (c) .NET Foundation and Contributors. All rights reserved. Licensed under the University of Illinois/NCSA Open Source License. See LICENSE.txt in the project root for license information.

using System;
using ClangSharp.Interop;

namespace ClangSharp
{
    public sealed class VariableArrayType : ArrayType
    {
        private readonly Lazy<Expr> _sizeExpr;

        internal VariableArrayType(CXType handle) : base(handle, CXTypeKind.CXType_VariableArray, CX_TypeClass.CX_TypeClass_VariableArray)
        {
            _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.SizeExpr));
        }

        public Expr SizeExpr => _sizeExpr.Value;
    }
}
