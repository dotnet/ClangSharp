// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DependentVectorType : Type
{
    private ValueLazy<DependentVectorType, Type> _elementType;
    private ValueLazy<DependentVectorType, Expr> _sizeExpr;

    internal unsafe DependentVectorType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_DependentVector)
    {
        _elementType = new ValueLazy<DependentVectorType, Type>(&ElementTypeFactory);
        _sizeExpr = new ValueLazy<DependentVectorType, Expr>(&SizeExprFactory);
    }

    public Type ElementType => _elementType.GetValue(this);

    public long Size => Handle.ArraySize;

    public Expr SizeExpr => _sizeExpr.GetValue(this);

    private static unsafe Expr SizeExprFactory(DependentVectorType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.SizeExpr);

    private static unsafe Type ElementTypeFactory(DependentVectorType self) => self.TranslationUnit.GetOrCreate<Type>(self.Handle.ElementType);
}
