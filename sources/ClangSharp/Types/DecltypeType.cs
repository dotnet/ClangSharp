// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;

namespace ClangSharp;

public sealed class DecltypeType : Type
{
    private readonly Lazy<Expr> _underlyingExpr;
    private readonly Lazy<Type> _underlyingType;

    internal DecltypeType(CXType handle) : base(handle, CXTypeKind.CXType_Unexposed, CX_TypeClass.CX_TypeClass_Decltype)
    {
        _underlyingExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.UnderlyingExpr));
        _underlyingType = new Lazy<Type>(() => TranslationUnit.GetOrCreate<Type>(Handle.UnderlyingType));
    }

    public Expr UnderlyingExpr => _underlyingExpr.Value;

    public Type UnderlyingType => _underlyingType.Value;
}
