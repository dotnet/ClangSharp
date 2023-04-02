// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DependentSizedArrayType : ArrayType
{
    private readonly Lazy<Expr> _sizeExpr;

    internal DependentSizedArrayType(CXType handle) : base(handle, CXType_DependentSizedArray, CX_TypeClass_DependentSizedArray)
    {
        _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SizeExpr));
    }

    public Expr SizeExpr => _sizeExpr.Value;
}
