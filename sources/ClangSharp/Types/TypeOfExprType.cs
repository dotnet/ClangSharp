// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class TypeOfExprType : Type
{
    private readonly Lazy<Expr> _underlyingExpr;

    internal TypeOfExprType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_TypeOfExpr)
    {
        _underlyingExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.UnderlyingExpr));
    }

    public Expr UnderlyingExpr => _underlyingExpr.Value;
}
