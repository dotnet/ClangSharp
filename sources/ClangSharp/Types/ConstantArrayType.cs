// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public class ConstantArrayType : ArrayType
{
    private ValueLazy<ConstantArrayType, Expr> _sizeExpr;

    internal ConstantArrayType(CXType handle) : this(handle, CXType_ConstantArray, CX_TypeClass_ConstantArray)
    {
    }

    private protected unsafe ConstantArrayType(CXType handle, CXTypeKind expectedTypeKind, CX_TypeClass expectedTypeClass) : base(handle, expectedTypeKind, expectedTypeClass)
    {
        _sizeExpr = new ValueLazy<ConstantArrayType, Expr>(&SizeExprFactory);
    }

    public long Size => Handle.ArraySize;

    public Expr SizeExpr => _sizeExpr.GetValue(this);

    private static unsafe Expr SizeExprFactory(ConstantArrayType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.SizeExpr);
}
