// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DependentSizedArrayType : ArrayType
{
    private ValueLazy<DependentSizedArrayType, Expr> _sizeExpr;

    internal unsafe DependentSizedArrayType(CXType handle) : base(handle, CXType_DependentSizedArray, CX_TypeClass_DependentSizedArray)
    {
        _sizeExpr = new ValueLazy<DependentSizedArrayType, Expr>(&SizeExprFactory);
    }

    public Expr SizeExpr => _sizeExpr.GetValue(this);

    private static unsafe Expr SizeExprFactory(DependentSizedArrayType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.SizeExpr);
}
