// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DependentSizedExtVectorType : Type
{
    private readonly Lazy<Type> _elementType;
    private readonly Lazy<Expr> _sizeExpr;

    internal DependentSizedExtVectorType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_DependentSizedExtVector)
    {
        _elementType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.ElementType));
        _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SizeExpr));
    }

    public Type ElementType => _elementType.Value;

    public long Size => Handle.ArraySize;

    public Expr SizeExpr => _sizeExpr.Value;
}
