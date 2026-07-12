// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DecltypeType : Type
{
    private ValueLazy<DecltypeType, Expr> _underlyingExpr;
    private ValueLazy<DecltypeType, Type> _underlyingType;

    internal unsafe DecltypeType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_Decltype)
    {
        _underlyingExpr = new ValueLazy<DecltypeType, Expr>(&UnderlyingExprFactory);
        _underlyingType = new ValueLazy<DecltypeType, Type>(&UnderlyingTypeFactory);
    }

    public Expr UnderlyingExpr => _underlyingExpr.GetValue(this);

    public Type UnderlyingType => _underlyingType.GetValue(this);

    private static unsafe Type UnderlyingTypeFactory(DecltypeType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.UnderlyingType);

    private static unsafe Expr UnderlyingExprFactory(DecltypeType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.UnderlyingExpr);
}
