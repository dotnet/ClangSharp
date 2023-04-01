// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class VariableArrayType : ArrayType
{
    private readonly Lazy<Expr> _sizeExpr;

    internal VariableArrayType(CXType handle) : base(handle, CXType_VariableArray, CX_TypeClass_VariableArray)
    {
        _sizeExpr = new Lazy<Expr>(() => TranslationUnit.GetOrCreate<Expr>(handle.SizeExpr));
    }

    public Expr SizeExpr => _sizeExpr.Value;
}
