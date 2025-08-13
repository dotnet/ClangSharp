// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class ConstantArrayType : ArrayType
{
    private readonly ValueLazy<Expr> _sizeExpr;

    internal ConstantArrayType(CXType handle) : base(handle, CXType_ConstantArray, CX_TypeClass_ConstantArray)
    {
        _sizeExpr = new ValueLazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(Handle.SizeExpr));
    }

    public long Size => Handle.ArraySize;

    public Expr SizeExpr => _sizeExpr.Value;
}
