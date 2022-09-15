// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class DependentSizedArrayType : ArrayType
{
    private readonly Lazy<Expr> _sizeExpr;

    internal DependentSizedArrayType(CXType handle) : base(handle, CXTypeKind.CXType_DependentSizedArray, CX_TypeClass.CX_TypeClass_DependentSizedArray)
    {
        _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SizeExpr));
    }

    public Expr SizeExpr => _sizeExpr.Value;
}
