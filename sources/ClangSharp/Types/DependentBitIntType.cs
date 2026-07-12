// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class DependentBitIntType : Type
{
    private ValueLazy<DependentBitIntType, Expr> _numBitsExpr;

    internal unsafe DependentBitIntType(CXType handle) : base(handle, CXType_Unexposed, CX_TypeClass_DependentBitInt)
    {
        _numBitsExpr = new ValueLazy<DependentBitIntType, Expr>(&NumBitsExprFactory);
    }

    public bool IsSigned => Handle.IsSigned;

    public bool IsUnsigned => Handle.IsUnsigned;

    public Expr NumBitsExpr => _numBitsExpr.GetValue(this);

    private static unsafe Expr NumBitsExprFactory(DependentBitIntType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.NumBitsExpr);
}
