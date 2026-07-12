// Copyright (c) .NET Foundation and Contributors. All Rights Reserved. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Diagnostics;
using ClangSharp.Interop;
using static ClangSharp.Interop.CXCursorKind;
using static ClangSharp.Interop.CX_StmtClass;

namespace ClangSharp;

public sealed class BinaryConditionalOperator : AbstractConditionalOperator
{
    private ValueLazy<BinaryConditionalOperator, OpaqueValueExpr> _opaqueValue;

    internal unsafe BinaryConditionalOperator(CXCursor handle) : base(handle, CXCursor_UnexposedExpr, CX_StmtClass_BinaryConditionalOperator)
    {
        Debug.Assert(NumChildren is 4);
        _opaqueValue = new ValueLazy<BinaryConditionalOperator, OpaqueValueExpr>(&OpaqueValueFactory);
    }

    public Expr Common => (Expr)Children[0];

    public new Expr Cond => (Expr)Children[1];

    public new Expr FalseExpr => (Expr)Children[3];

    public OpaqueValueExpr OpaqueValue => _opaqueValue.GetValue(this);

    public new Expr TrueExpr => (Expr)Children[2];

    private static unsafe OpaqueValueExpr OpaqueValueFactory(BinaryConditionalOperator self) => self.TranslationUnit.GetOrCreate<OpaqueValueExpr>(self.Handle.OpaqueValue);
}
