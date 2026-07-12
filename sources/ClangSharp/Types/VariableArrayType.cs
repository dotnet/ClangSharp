// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using ClangSharp.Interop;
using static ClangSharp.Interop.CXTypeKind;
using static ClangSharp.Interop.CX_TypeClass;

namespace ClangSharp;

public sealed class VariableArrayType : ArrayType
{
    private ValueLazy<VariableArrayType, Expr> _sizeExpr;

    internal unsafe VariableArrayType(CXType handle) : base(handle, CXType_VariableArray, CX_TypeClass_VariableArray)
    {
        _sizeExpr = new ValueLazy<VariableArrayType, Expr>(&SizeExprFactory);
    }

    public Expr SizeExpr => _sizeExpr.GetValue(this);

    private static unsafe Expr SizeExprFactory(VariableArrayType self) => self.TranslationUnit.GetOrCreate<Expr>(self.Handle.SizeExpr);
}
