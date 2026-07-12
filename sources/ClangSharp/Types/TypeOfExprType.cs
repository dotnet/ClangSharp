// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class TypeOfExprType : Type
{
    private ValueLazy<TypeOfExprType, Expr> _underlyingExpr;

    internal unsafe TypeOfExprType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_TypeOfExpr)
    {
        _underlyingExpr = new ValueLazy<TypeOfExprType, Expr>(&UnderlyingExprFactory);
    }

    public Expr UnderlyingExpr => _underlyingExpr.GetValue(this);

    private static unsafe Expr UnderlyingExprFactory(TypeOfExprType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.UnderlyingExpr);
}
